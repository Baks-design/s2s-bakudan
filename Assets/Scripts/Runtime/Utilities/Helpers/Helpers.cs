using UnityEngine;

namespace Game.Runtime.Utilities.Helpers
{
    public static class Helpers
    {
        public static void LockCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false; 
        }

        public static void UnlockCursor()
        {
            Cursor.lockState = CursorLockMode.None; 
            Cursor.visible = true; 
        }

        public static bool IsInLayerMask(GameObject obj, LayerMask layerMask) => (layerMask.value & (1 << obj.layer)) != 0;
    }
}