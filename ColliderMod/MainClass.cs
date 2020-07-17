using MelonLoader;
using UnityEngine;

namespace ColliderMod
{
    public class MainClass : MelonMod
    {
        private const KeyCode ToggleClickedCollider = KeyCode.N;
        private const KeyCode RenableAllColliders = KeyCode.M;

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
        }
    }
}
