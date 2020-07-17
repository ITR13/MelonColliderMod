﻿using System.Collections.Generic;
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
        public static readonly HashSet<int> MyRenderers = new HashSet<int>();

        private static readonly List<Sphere> SphereCache = new List<Sphere>();
        private static readonly List<Cube> CubeCache = new List<Cube>();
        private static readonly List<Capsule> CapsuleCache = new List<Capsule>();

        private static readonly SphereColliderList SphereColliders = new SphereColliderList();
        private static readonly BoxColliderList BoxColliders = new BoxColliderList();
        private static readonly CapsuleColliderList CapsuleColliders = new CapsuleColliderList();

        public static void RegenerateAll()
        {
            var oldSphereCount = SphereColliders.Count;
            var oldBoxCount = BoxColliders.Count;
            var oldCapsuleCount = CapsuleColliders.Count;

            GetAllColliders(SphereColliders);
            GetAllColliders(BoxColliders);
            GetAllColliders(CapsuleColliders);

            Regenerate(SphereCache, oldSphereCount, SphereColliders);
            Regenerate(CubeCache, oldBoxCount, BoxColliders);
            Regenerate(CapsuleCache, oldCapsuleCount, CapsuleColliders);

            MelonModLogger.Log(
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
                if (colliders[i] == null)
                {
                    colliders.RemoveAt(i);
                    cache[colliders.Count].Enabled = false;
                    continue;
                }

                cache[j].Update(colliders[i]);
            }
        }

        public static void DisableAll()
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

            MelonModLogger.Log(
                $"No longer showing {oldSphereCount} sphere colliders, {oldBoxCount} box colliders, and {oldCapsuleCount} capsule colliders"
            );
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

            public Sphere()
            {
                var sphereObject =
                    GameObject.CreatePrimitive(PrimitiveType.Sphere);
                Object.DontDestroyOnLoad(sphereObject);
                Object.Destroy(sphereObject.GetComponent<Collider>());
                _transform = sphereObject.transform;

                MyRenderers.Add((int)sphereObject.GetComponent<Renderer>().GetCachedPtr());
            }

            private static float Max(float a, float b, float c)
            {
                return a > b
                    ? a > c ? a : c
                    : b > c ? b : c;
            }

            public void Update(SphereCollider collider)
            {
                if (!_transform.gameObject.activeSelf)
                {
                    _transform.gameObject.SetActive(true);
                }

                var t = collider.transform;
                var ls = t.lossyScale;

                var diameter = collider.radius * Max(ls.x, ls.y, ls.z) * 2;
                var position = t.TransformPoint(collider.center);

                _transform.localScale = Vector3.one * diameter;
                _transform.position = position;
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

            public Cube()
            {
                var cubeObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Object.DontDestroyOnLoad(cubeObject);
                Object.Destroy(cubeObject.GetComponent<Collider>());
                _transform = cubeObject.transform;

                MyRenderers.Add((int)cubeObject.GetComponent<Renderer>().GetCachedPtr());
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

                MyRenderers.Add((int)_topSphere.GetComponent<Renderer>().GetCachedPtr());
                MyRenderers.Add((int)_bottomSphere.GetComponent<Renderer>().GetCachedPtr());
                MyRenderers.Add((int)_middleCylinder.GetComponent<Renderer>().GetCachedPtr());
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
            }
        }
    }
}