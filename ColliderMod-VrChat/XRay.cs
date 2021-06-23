using System.Collections.Generic;
using MelonLoader;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ColliderMod
{
    static class XRay
    {
        private static Dictionary<int, Renderer> OriginallyEnabled { get; } =
            new Dictionary<int, Renderer>();

        private static Dictionary<int, Renderer> OriginallyDisabled { get; } =
            new Dictionary<int, Renderer>();

        public static void ToggleEnabledRenderers()
        {
            ToggleRenderers(OriginallyEnabled, true);
        }

        public static void ToggleDisabledRenderers()
        {
            ToggleRenderers(OriginallyDisabled, false);
        }

        private static void ToggleRenderers(
            IDictionary<int, Renderer> mutableRendererCollection,
            bool toggleEnabled
        )
        {
            var word = toggleEnabled ? "enabled" : "disabled";

            if (mutableRendererCollection.Count != 0)
            {
                MelonLogger.Msg($"Setting {mutableRendererCollection.Count} renderers back to {toggleEnabled}");
                foreach (var renderer in mutableRendererCollection.Values)
                {
                    if (renderer == null) continue;
                    renderer.enabled = toggleEnabled;
                }

                mutableRendererCollection.Clear();
                return;
            }

            var count = 0;
            foreach (var renderer in AllRenderers())
            {
                if (renderer.enabled != toggleEnabled) continue;
                var ptr = (int)renderer.GetCachedPtr();

                if (OriginallyEnabled.ContainsKey(ptr)) continue;
                if (OriginallyDisabled.ContainsKey(ptr)) continue;
                if(ColliderDisplay.MyRenderers.Contains(ptr)) continue;

                mutableRendererCollection.Add(ptr, renderer);
                renderer.enabled = !toggleEnabled;
                count++;
            }

            MelonLogger.Msg($"Toggled {count} {word} renderers");
        }

        private static List<Renderer> AllRenderers()
        {
            var mutableList = new List<Renderer>();
            var sceneCount = SceneManager.sceneCount;
            for (var sceneIndex = 0; sceneIndex < sceneCount; sceneIndex++)
            {
                var scene = SceneManager.GetSceneAt(sceneIndex);
                var rootObjects = scene.GetRootGameObjects();
                foreach (var rootObject in rootObjects)
                {
                    IterateObject(rootObject, mutableList);
                }
            }

            return mutableList;
        }

        private static void IterateObject(
            GameObject go,
            List<Renderer> mutableRendererCollection
        )
        {
            var renderer = go.GetComponent<Renderer>();
            if (renderer != null)
            {
                mutableRendererCollection.Add(renderer);
            }

            var childCount = go.transform.childCount;
            for (var childIndex = 0; childIndex < childCount; childIndex++)
            {
                var child = go.transform.GetChild(childIndex);
                IterateObject(child.gameObject, mutableRendererCollection);
            }
        }
    }
}
