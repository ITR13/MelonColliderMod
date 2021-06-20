using Il2CppSystem.Collections.Generic;
using MelonLoader;
using UnityEngine;

namespace ColliderMod
{
    static class ColliderToggler
    {
        private static readonly List<Collider> ToggledColliders = new List<Collider>();

        private static Collider _createdCollider;
        private static Transform _createdColliderTransform;
        private static Vector3 _createdColliderPosition;
        public static Renderer CreatedColliderRenderer { get; private set; }

        public static void OnSceneLoaded()
        {
            ReenableAll(false);

            if (_createdColliderTransform == null)
            {
                var createdCollider = GameObject.CreatePrimitive(PrimitiveType.Cube);
                _createdColliderTransform = createdCollider.transform;
                _createdCollider = createdCollider.GetComponent<Collider>();
                CreatedColliderRenderer = createdCollider.GetComponent<Renderer>();

                ColliderDisplay.OnSceneLoaded();

                createdCollider.gameObject.SetActive(false);

                if (ColliderDisplay.CreatedColliderMaterial != null)
                {
                    CreatedColliderRenderer.sharedMaterial = ColliderDisplay.CreatedColliderMaterial;
                }
            }


            UpdateCreatedCollider();
        }

        public static void ReenableAll(bool silent=false)
        {
            var reenabled = 0;
            var skipped = 0;
            foreach (var collider in ToggledColliders)
            {
                if (collider == null)
                {
                    skipped++;
                    continue;
                }

                reenabled++;
                collider.enabled = true;
            }

            ToggledColliders.Clear();

            if (!silent)
            {
                MelonLogger.Msg($"Reenabled {reenabled} colliders, skipped {skipped} colliders");
            }
        }

        public static void ToggleForwardCollider()
        {
            var camera = Camera.main;
            if (camera == null)
            {
                MelonLogger.Warning("No camera found, cannot toggle");
                return;
            }

            var cameraRay = camera.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(cameraRay, out var hitInfo)) return;

            if (hitInfo.collider == null)
            {
                MelonLogger.Warning("Clicked collider has no collider?");
                return;
            }

            if (hitInfo.collider == _createdCollider)
            {
                MelonLogger.Msg("Ignoring clicked collider since it's the created one");
                return;
            }

            ToggleCollider(hitInfo.collider);
        }

        private static void ToggleCollider(Collider collider)
        {
            if (collider == null) return;
            collider.enabled = false;
            ToggledColliders.Add(collider);

            var name = GetColliderName(collider);
            MelonLogger.Msg($"Toggled collider {name}");
        }

        private static string GetColliderName(Collider collider)
        {
            var transform = collider.transform;
            var names = new System.Collections.Generic.List<string>
            {
                collider.GetType().Name
            };

            do
            {
                names.Add(transform.gameObject.name);
                transform = transform.parent;
            } while (transform != null);

            names.Reverse();
            return string.Join("/", names);
        }

        public static void CreateOrMoveCollider()
        {
            var camera = Camera.main;
            if (camera == null)
            {
                MelonLogger.Warning("No camera found, cannot toggle");
                return;
            }

            if (_createdColliderTransform == null)
            {
                MelonLogger.Warning("Created collider not yet created");
                return;
            }

            _createdColliderTransform.gameObject.SetActive(true);

            var cameraRay = camera.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(cameraRay, out var hitInfo))
            {
                _createdColliderPosition = camera.transform.position +  camera.transform.forward * 5;
            }
            else
            {
                _createdColliderPosition = hitInfo.point;
            }
            MelonLogger.Msg($"Setting root created collider position to {_createdColliderPosition[0]},{_createdColliderPosition[1]},{_createdColliderPosition[2]}");

            UpdateCreatedCollider();
        }

        public static void UpdateCreatedCollider()
        {
            if(_createdColliderTransform==null||!_createdColliderTransform.gameObject.activeSelf) return;
                
            Vector3 A2V(float[] arr) => new Vector3(arr[0], arr[1], arr[2]);
            _createdColliderTransform.localScale = A2V(ConfigWatcher.ColliderModConfig.createdColliderSize);
            _createdColliderTransform.position =
                _createdColliderPosition + A2V(ConfigWatcher.ColliderModConfig.createdColliderOffset);

            if (ColliderDisplay.CreatedColliderMaterial != null)
            {
                CreatedColliderRenderer.sharedMaterial = ColliderDisplay.CreatedColliderMaterial;
            }
            MelonLogger.Msg($"Created collider is at {_createdColliderTransform.position[0]},{_createdColliderTransform.position[1]},{_createdColliderTransform.position[2]} with size {_createdColliderTransform.localScale[0]},{_createdColliderTransform.localScale[1]},{_createdColliderTransform.localScale[2]}");
        }

        public static void RemoveCreatedCollider()
        {
            if(_createdColliderTransform==null) return;
            _createdColliderTransform.gameObject.SetActive(false);
        }
    }
}
