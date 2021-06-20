using System;
using UnityEngine;

namespace ColliderMod
{
    [Serializable]
    public class ColliderModConfig
    {
        public string defaultShader = "Legacy Shaders/Transparent/VertexLit";

        public KeyBinding toggleClickedCollider = new KeyBinding(KeyCode.LeftAlt, KeyCode.Mouse0);
        public KeyBinding reenableAllColliders = new KeyBinding(KeyCode.LeftAlt, KeyCode.T);
        public KeyBinding toggleXRay = new KeyBinding(KeyCode.LeftAlt, KeyCode.F);
        public KeyBinding toggleInvisSee = new KeyBinding(KeyCode.None, KeyCode.None);

        public KeyBinding regenAllColliderDisplays = new KeyBinding(KeyCode.LeftAlt, KeyCode.E);
        public KeyBinding disableAllColliderDisplays = new KeyBinding(KeyCode.LeftAlt, KeyCode.R);
        public KeyBinding updateAllColliderDisplays = new KeyBinding(KeyCode.None, KeyCode.None);


        public KeyBinding createColliderAt = new KeyBinding(KeyCode.None, KeyCode.None);
        public KeyBinding removeCreatedCollider = new KeyBinding(KeyCode.None, KeyCode.None);
        public float[] createdColliderSize = { 1f, 0.2f, 1f };
        public float[] createdColliderOffset = { 0, 0f, 0f };
        public float[] createdColliderColor = {1, 1, 1, 0.4f};
    }

    [Serializable]
    public class KeyBinding
    {
        public KeyCode hold, trigger;

        public KeyBinding() { }

        public KeyBinding(KeyCode hold, KeyCode trigger)
        {
            this.hold = hold;
            this.trigger = trigger;
        }

        public bool Active()
        {
            return trigger != KeyCode.None &&
                   (hold == KeyCode.None || Input.GetKey(hold)) &&
                   Input.GetKeyDown(trigger);
        }
    }
}
