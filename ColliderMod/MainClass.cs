using MelonLoader;

namespace ColliderMod
{
    public class MainClass : MelonMod
    {
        private static ColliderModConfig Config => ConfigWatcher.ColliderModConfig;

        private static bool ToggleClickedCollider => Config.toggleClickedCollider.Active();
        private static bool ReenableAllColliders => Config.reenableAllColliders.Active();
        private static bool ToggleXRay => Config.toggleXRay.Active();
        private static bool ToggleInvisSee => Config.toggleInvisSee.Active();


        private static bool RegenAllColliderDisplays => Config.regenAllColliderDisplays.Active();
        private static bool DisableAllColliderDisplays => Config.disableAllColliderDisplays.Active();
        private static bool UpdateAllColliderDisplays => Config.updateAllColliderDisplays.Active();



        public override void OnApplicationQuit()
        {
            ConfigWatcher.Unload();
        }

        public override void OnUpdate()
        {
            ConfigWatcher.UpdateIfDirty();

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
        }
    }
}
