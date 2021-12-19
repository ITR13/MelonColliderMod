using MelonLoader;
using System;

namespace ColliderMod
{
    public class MainClass : MelonMod
    {
        public static Action<string> Msg;
        public static Action<string> Warning;
        public static Action<string> Error;

        private static ColliderModConfig Config => ConfigWatcher.ColliderModConfig;

        private static bool ToggleClickedCollider => Config.toggleClickedCollider.Active();
        private static bool ReenableAllColliders => Config.reenableAllColliders.Active();
        private static bool ToggleXRay => Config.toggleXRay.Active();
        private static bool ToggleInvisSee => Config.toggleInvisSee.Active();


        private static bool RegenAllColliderDisplays => Config.regenAllColliderDisplays.Active();
        private static bool DisableAllColliderDisplays => Config.disableAllColliderDisplays.Active();
        private static bool UpdateAllColliderDisplays => Config.updateAllColliderDisplays.Active();

        private static bool CreateColliderAt => Config.createColliderAt.Active();
        private static bool RemoveCreatedCollider => Config.removeCreatedCollider.Active();

        public static bool ForceDisable;

        public override void OnApplicationStart()
        {
            Msg = LoggerInstance.Msg;
            Warning = LoggerInstance.Warning;
            Error = LoggerInstance.Error;
#if VRCHAT
            WorldCheck.Init();
#endif
        }

        public override void OnApplicationQuit()
        {
            ConfigWatcher.Unload();
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            // NB: toggler needs to be after display because it uses material from display
            ColliderDisplay.OnSceneLoaded();
            ColliderToggler.OnSceneLoaded();
        }

        public override void OnUpdate()
        {
            if (ForceDisable) return;

            if (ConfigWatcher.UpdateIfDirty())
            {
                ColliderToggler.UpdateCreatedCollider();
                ColliderDisplay.UpdateColors();
            }

            if (RemoveCreatedCollider)
            {
                ColliderToggler.RemoveCreatedCollider();
            }

            if (ToggleClickedCollider)
            {
                ColliderToggler.ToggleForwardCollider();
                ColliderDisplay.UpdateAll();
            }

            if (ReenableAllColliders)
            {
                ColliderToggler.ReenableAll();
            }

            if (ToggleXRay)
            {
                XRay.ToggleEnabledRenderers();
            }

            if (ToggleInvisSee)
            {
                XRay.ToggleDisabledRenderers();
            }

            if (RegenAllColliderDisplays)
            {
                ColliderDisplay.RegenerateAll();
                ColliderDisplay.UpdateAll();
            }

            if (DisableAllColliderDisplays)
            {
                ColliderDisplay.DisableAll();
            }

            if (UpdateAllColliderDisplays)
            {
                ColliderDisplay.UpdateAll();
            }

            if (CreateColliderAt)
            {
                ColliderToggler.CreateOrMoveCollider();
            }
        }
    }
}
