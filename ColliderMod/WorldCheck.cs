using ColliderMod;
using VRChatUtilityKit.Utilities;

namespace ColliderMod
{
    // Mostly stolen from https://github.com/loukylor/VRC-Mods/blob/main/TriggerESP/TriggerESPMod.cs#L82-L89
    internal static class WorldCheck
    {
        public static void Init()
        {
            VRCUtils.OnEmmWorldCheckCompleted += areRiskyFuncsAllowed =>
            {
                MainClass.ForceDisable = !areRiskyFuncsAllowed;
                if (!MainClass.ForceDisable)
                {
                    ColliderDisplay.OnSceneLoaded();
                    ColliderToggler.OnSceneLoaded();
                }
            };
        }
    }
}