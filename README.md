***NB: Not tested in VR. Only affects your local instance.***

### What does this do
This allows you to see hidden renderers, and visualizes sphere colliders, box colliders, and capsule colliders.  
Solid colliders are green, and trigger colliders are red.  
It also allows you to disable any colliders, even those not shown by this mod.  

### How to use
When you first run a game after installing the mod it will create a file in "\[GameRoot\]\UserData\ColliderModConfig.json".  
The keybindings use "hold" for the key you need to hold to trigger something, and "trigger" for the key you need to press while hold is held.   
If hold is None, only trigger will be used. If trigger is None, the binding is disabled.  
Check [The Unity Docs](https://docs.unity3d.com/ScriptReference/KeyCode.html#Properties) for a list of keybindings.  

**DefaultShader**: What shader to use for the collider displays.  

**toggleClickedCollider**: Disables whatever collider you're looking at. (Default: LAlt+LMouse)  
**reenableAllColliders**: Renables previously disabled colliders. (Default: LAlt+R)  
**toggleXRay**: Disables all renderers except UI and those affected by other features of this mod. (Default: LAlt+F)  
**toggleInvisSee**: Enables renderers that were originally disabled (Default: Disabled)  
**regenAllColliderDisplays**: Visualizes all enabled colliders in the world. Really laggy in big worlds. (Default: LAlt+E)..
**disableAllColliderDisplays**: Hides all visualized colliders. (Default: LAlt+T)  
**updateAllColliderDisplays**: Updates the position of all current visualizations and removes the ones that have dissappeared.. (Default: Disabled)

### More notes
Update all collider displays is called automatically when you toggle a collider and regen all colliders.  
It's less laggy than regening all colliders, but doesn't show new colliders.  


Toggling a collider logs what collider is toggled into the console.  
It might not toggle what you expect, so you might have to use the command multiple times to disable the right one. ​ 