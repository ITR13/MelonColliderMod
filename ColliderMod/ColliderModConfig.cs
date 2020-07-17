using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ColliderMod
{
    [Serializable]
    public class ColliderModConfig
    {
        public KeyBinding toggleClickedCollider = new KeyBinding(KeyCode.LeftAlt, KeyCode.Mouse0);
        public KeyBinding reenableAllColliders = new KeyBinding(KeyCode.LeftAlt, KeyCode.T);
        public KeyBinding toggleXRay = new KeyBinding(KeyCode.LeftAlt, KeyCode.F);
        public KeyBinding toggleInvisSee = new KeyBinding(KeyCode.None, KeyCode.None);

        public KeyBinding regenAllColliderDisplays = new KeyBinding(KeyCode.LeftAlt, KeyCode.E);
        public KeyBinding disableAllColliderDisplays = new KeyBinding(KeyCode.LeftAlt, KeyCode.R);
        public KeyBinding updateAllColliderDisplays = new KeyBinding(KeyCode.None, KeyCode.None);
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
