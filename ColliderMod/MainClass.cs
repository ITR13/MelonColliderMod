using MelonLoader;
using UnityEngine;

namespace ColliderMod
{
    public class MainClass : MelonMod
    {
        private const KeyCode ToggleClickedCollider = KeyCode.N;
        private const KeyCode RenableAllColliders = KeyCode.M;
        private const KeyCode ToggleXRay = KeyCode.V;
        private const KeyCode ToggleInvisSee = KeyCode.B;

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(ToggleClickedCollider))
            {
                ColliderToggler.ToggleForwardCollider();
            }

            if (Input.GetKeyDown(RenableAllColliders))
            {
                ColliderToggler.ReenableAll();
            }

            if (Input.GetKeyDown(ToggleXRay))
            {
                XRay.ToggleEnabledRenderers();
            }

            if (Input.GetKeyDown(ToggleInvisSee))
            {
                XRay.ToggleDisabledRenderers();
            }
        }
    }
}
