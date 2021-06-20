using System.Collections.Generic;
using System.Linq;
using MelonLoader;
using UnityEngine;
using UnityEngine.SceneManagement;
using SphereColliderList = Il2CppSystem.Collections.Generic.List<UnityEngine.SphereCollider>;
using BoxColliderList = Il2CppSystem.Collections.Generic.List<UnityEngine.BoxCollider>;
using CapsuleColliderList = Il2CppSystem.Collections.Generic.List<UnityEngine.CapsuleCollider>;

namespace ColliderMod
{
    public static class ColliderDisplay
    {
        private static string ShaderName =>
            ConfigWatcher.ColliderModConfig.defaultShader;

        public static readonly HashSet<int> MyRenderers = new HashSet<int>();

        private static readonly List<Sphere> SphereCache = new List<Sphere>();
        private static readonly List<Cube> CubeCache = new List<Cube>();
        private static readonly List<Capsule> CapsuleCache = new List<Capsule>();

        private static readonly SphereColliderList SphereColliders = new SphereColliderList();
        private static readonly BoxColliderList BoxColliders = new BoxColliderList();
        private static readonly CapsuleColliderList CapsuleColliders = new CapsuleColliderList();

        private static Material _triggerMaterial;
        private static Material _solidMaterial;
        public static Material CreatedColliderMaterial { get; private set; }

        public static void OnSceneLoaded()
        {
            CreateMaterials();
            DisableAll(false);
        }

        private static void CreateMaterials()
        {
#if FindObjectsOfTypeAll
            var shaders = Resources.FindObjectsOfTypeAll<Shader>();
#else
            MelonLogger.Msg("Temporary fix for not being able to use Resources.FindObjectsOfTypeAll");
            var shaders = new Shader[0];
#endif
            var selectedShader = Shader.Find(ShaderName);

            if (selectedShader == null)
            {
                MelonLogger.Error(
                    $"Failed to find shader with name {ShaderName}. Valid shaders:\n" +
                    string.Join("\n", shaders.Select(shader => shader.name))
                );
                selectedShader = shaders.FirstOrDefault(
                    shader => shader.isSupported && shader.name.Contains("Transparent")
                );
            }

            if (selectedShader == null)
            {
                MelonLogger.Error(
                    "Failed to find transparent shader for colliders"
                );
                selectedShader = shaders.FirstOrDefault();
            }
            else
            {
                MelonLogger.Msg(
                    "Creating material with shader " + selectedShader.name
                );
            }

            _triggerMaterial = new Material(selectedShader);
            _solidMaterial = new Material(selectedShader);
            CreatedColliderMaterial = new Material(selectedShader);

            UpdateColors();
            Resources.UnloadUnusedAssets();
        }

        public static void UpdateColors()
        {
            Color A2C(float[] arr) => new Color(arr[0], arr[1], arr[2], arr[3]);
            if (_triggerMaterial != null) _triggerMaterial.color = A2C(ConfigWatcher.ColliderModConfig.triggerColliderColor);
            if (_solidMaterial != null) _solidMaterial.color = A2C(ConfigWatcher.ColliderModConfig.solidColliderColor);

            if (CreatedColliderMaterial == null) return;
            CreatedColliderMaterial.color = A2C(ConfigWatcher.ColliderModConfig.createdColliderColor);
            if (ColliderToggler.CreatedColliderRenderer != null)
            {
                ColliderToggler.CreatedColliderRenderer.sharedMaterial = CreatedColliderMaterial;
            }
        }

        public static void RegenerateAll()
        {
            CreateMaterials();

            var oldSphereCount = SphereColliders.Count;
            var oldBoxCount = BoxColliders.Count;
            var oldCapsuleCount = CapsuleColliders.Count;

            GetAllColliders(SphereColliders);
            GetAllColliders(BoxColliders);
            GetAllColliders(CapsuleColliders);

            Regenerate(SphereCache, oldSphereCount, SphereColliders);
            Regenerate(CubeCache, oldBoxCount, BoxColliders);
            Regenerate(CapsuleCache, oldCapsuleCount, CapsuleColliders);

            MelonLogger.Msg(
                $"Showing {SphereColliders.Count} sphere colliders, {BoxColliders.Count} box colliders, and {CapsuleColliders.Count} capsule colliders"
            );
        }

        private static void GetAllColliders<T>(Il2CppSystem.Collections.Generic.List<T> colliderList)
        {
            colliderList.Clear();
            var sceneCount = SceneManager.sceneCount;
            for (var i = 0; i < sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                var rootGameObjects = scene.GetRootGameObjects();
                foreach (var gameObject in rootGameObjects)
                {
                    var colliders =
                        gameObject.GetComponentsInChildren<T>();
                    foreach (var collider in colliders)
                    {
                        colliderList.Add(collider);
                    }
                }
            }
        }

        private static void Regenerate
            <T, TSelf>(IList<TSelf> cache, int oldCount, Il2CppSystem.Collections.Generic.List<T> colliders)
            where TSelf : class, IDisplay<T, TSelf>, new() where T : Collider
        {
            if (colliders.Count < oldCount)
            {
                for (var i = colliders.Count; i < oldCount; i++)
                {
                    cache[i].Enabled = false;
                }
                return;
            }

            if (colliders.Count <= cache.Count)
            {
                for (var i = oldCount; i < colliders.Count; i++)
                {
                    cache[i].Enabled = true;
                }
                return;
            }

            for (var i = oldCount; i < cache.Count; i++)
            {
                cache[i].Enabled = true;
            }

            while (cache.Count < colliders.Count)
            {
                cache.Add(new TSelf());
            }
        }

        public static void UpdateAll()
        {
            Update(SphereCache, SphereColliders);
            Update(CubeCache, BoxColliders);
            Update(CapsuleCache, CapsuleColliders);
        }

        private static void Update
            <T, TSelf>(IList<TSelf> cache, Il2CppSystem.Collections.Generic.List<T> colliders)
            where TSelf : class, IDisplay<T, TSelf>, new() where T : Collider
        {
            var j = 0;
            for (var i = colliders.Count - 1; i >= 0; i--)
            {
                if (colliders[i] == null || !colliders[i].enabled)
                {
                    colliders.RemoveAt(i);
                    cache[colliders.Count].Enabled = false;
                    continue;
                }

                cache[j++].Update(colliders[i]);
            }
        }

        public static void DisableAll(bool silent = false)
        {
            var oldSphereCount = SphereColliders.Count;
            var oldBoxCount = BoxColliders.Count;
            var oldCapsuleCount = CapsuleColliders.Count;

            SphereColliders.Clear();
            BoxColliders.Clear();
            CapsuleColliders.Clear();

            Regenerate(SphereCache, oldSphereCount, SphereColliders);
            Regenerate(CubeCache, oldBoxCount, BoxColliders);
            Regenerate(CapsuleCache, oldCapsuleCount, CapsuleColliders);

            if (!silent)
            {
                MelonLogger.Msg(
                    $"No longer showing {oldSphereCount} sphere colliders, {oldBoxCount} box colliders, and {oldCapsuleCount} capsule colliders"
                );
            }
        }

        private interface IDisplay<in T, TSelf>
            where TSelf : class, IDisplay<T, TSelf>, new()
            where T : Collider
        {
            bool Enabled { get; set; }
            void Update(T collider);
        }

        private class Sphere : IDisplay<SphereCollider, Sphere>
        {
            public bool Enabled
            {
                get => _transform.gameObject.activeSelf;
                set => _transform.gameObject.SetActive(value);
            }

            private readonly Transform _transform;
            private readonly Renderer _renderer;

            public Sphere()
            {
                var sphereObject =
                    GameObject.CreatePrimitive(PrimitiveType.Sphere);
                Object.DontDestroyOnLoad(sphereObject);
                Object.Destroy(sphereObject.GetComponent<Collider>());
                _transform = sphereObject.transform;

                _renderer = sphereObject.GetComponent<Renderer>();
                MyRenderers.Add((int)_renderer.GetCachedPtr());
            }

            private static float Max(float a, float b, float c)
            {
                return a > b
                    ? a > c ? a : c
                    : b > c ? b : c;
            }

            public void Update(SphereCollider collider)
            {
                var t = collider.transform;
                var ls = t.lossyScale;

                var diameter = collider.radius * Max(ls.x, ls.y, ls.z) * 2;
                var position = t.TransformPoint(collider.center);

                _transform.localScale = Vector3.one * diameter;
                _transform.position = position;

                _renderer.sharedMaterial = collider.isTrigger
                    ? _triggerMaterial
                    : _solidMaterial;
            }
        }

        private class Cube : IDisplay<BoxCollider, Cube>
        {
            public bool Enabled
            {
                get => _transform.gameObject.activeSelf;
                set => _transform.gameObject.SetActive(value);
            }

            private readonly Transform _transform;
            private readonly Renderer _renderer;

            public Cube()
            {
                var cubeObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Object.DontDestroyOnLoad(cubeObject);
                Object.Destroy(cubeObject.GetComponent<Collider>());
                _transform = cubeObject.transform;

                _renderer = cubeObject.GetComponent<Renderer>();
                MyRenderers.Add((int)_renderer.GetCachedPtr());
            }

            public void Update(BoxCollider collider)
            {
                var t = collider.transform;

                _transform.localScale = Vector3.Scale(
                    t.lossyScale,
                    collider.size
                );
                _transform.position = t.TransformPoint(collider.center);
                _transform.rotation = t.rotation;

                _renderer.sharedMaterial = collider.isTrigger
                    ? _triggerMaterial
                    : _solidMaterial;
            }
        }

        public class Capsule : IDisplay<CapsuleCollider, Capsule>
        {
            public bool Enabled
            {
                get => _parent.gameObject.activeSelf;
                set => _parent.gameObject.SetActive(value);
            }

            private readonly Transform _parent;
            private readonly Transform _topSphere;
            private readonly Transform _bottomSphere;
            private readonly Transform _middleCylinder;

            private readonly Renderer _topRenderer;
            private readonly Renderer _bottomRenderer;
            private readonly Renderer _middleRenderer;

            public Capsule()
            {
                _parent = new GameObject("Capsule").transform;
                Object.DontDestroyOnLoad(_parent);

                _topSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere)
                    .transform;
                _topSphere.parent = _parent;
                Object.Destroy(_topSphere.GetComponent<Collider>());

                _bottomSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere)
                    .transform;
                _bottomSphere.parent = _parent;
                Object.Destroy(_bottomSphere.GetComponent<Collider>());

                _middleCylinder = GameObject
                    .CreatePrimitive(PrimitiveType.Cylinder)
                    .transform;
                _middleCylinder.parent = _parent;
                Object.Destroy(_middleCylinder.GetComponent<Collider>());

                _bottomSphere.Rotate(180, 0, 0);


                _topRenderer = _topSphere.GetComponent<Renderer>();
                _bottomRenderer = _bottomSphere.GetComponent<Renderer>();
                _middleRenderer = _middleCylinder.GetComponent<Renderer>();

                MyRenderers.Add((int)_topRenderer.GetCachedPtr());
                MyRenderers.Add((int)_bottomRenderer.GetCachedPtr());
                MyRenderers.Add((int)_middleRenderer.GetCachedPtr());
            }

            private static float Max(float a, float b)
            {
                return a > b ? a : b;
            }

            public void Update(CapsuleCollider collider)
            {
                var t = collider.transform;
                var ls = t.lossyScale;
                var dir = collider.direction;

                var position = t.TransformPoint(collider.center);
                var rotation = t.rotation;
                var height = collider.height * ls[dir];
                var radius = collider.radius *
                             Max(
                                 ls[(dir + 1) % 3],
                                 ls[(dir + 2) % 3]
                             );

                switch (dir)
                {
                    case 0:
                        rotation *= new Quaternion(
                            0.707106781f,
                            -0.707106781f,
                            0,
                            0
                        );
                        break;
                    case 2:
                        rotation *= new Quaternion(
                            0.707106781f,
                            0,
                            0,
                            -0.707106781f
                        );
                        break;
                }

                var diameter = radius * 2;
                var midHeight = height / 2 - radius;


                _parent.position = position;
                _parent.rotation = rotation;

                _topSphere.localScale = Vector3.one * diameter;
                _topSphere.localPosition = Vector3.up * midHeight;

                _bottomSphere.localScale = Vector3.one * diameter;
                _bottomSphere.localPosition = -Vector3.up * midHeight;

                _middleCylinder.localScale = new Vector3(
                    diameter,
                    midHeight,
                    diameter
                );

                if (collider.isTrigger)
                {
                    _topRenderer.sharedMaterial = _triggerMaterial;
                    _bottomRenderer.sharedMaterial = _triggerMaterial;
                    _middleRenderer.sharedMaterial = _triggerMaterial;
                }
                else
                {
                    _topRenderer.sharedMaterial = _solidMaterial;
                    _bottomRenderer.sharedMaterial = _solidMaterial;
                    _middleRenderer.sharedMaterial = _solidMaterial;
                }
            }
        }
    }
}
