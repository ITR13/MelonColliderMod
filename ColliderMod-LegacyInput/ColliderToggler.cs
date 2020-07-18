using Il2CppSystem.Collections.Generic;
using MelonLoader;
using UnityEngine;

namespace ColliderMod
{
    static class ColliderToggler
    {
        private static readonly List<Collider> ToggledColliders = new List<Collider>();

        public static void ReenableAll()
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
            MelonModLogger.Log($"Reenabled {reenabled} colliders, skipped {skipped} colliders");
        }

        public static void ToggleForwardCollider()
        {
            var camera = Camera.main;
            if (camera == null)
            {
                MelonModLogger.LogWarning("No camera found, cannot toggle");
                return;
            }

            var cameraRay = camera.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(cameraRay, out var hitInfo)) return;

            if (hitInfo.collider == null)
            {
                MelonModLogger.LogWarning("Clicked collider has no collider?");
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
            MelonModLogger.Log($"Toggled collider {name}");
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
    }
}
