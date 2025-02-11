using UnityEngine;

namespace Game.Runtime.Utilities.Helpers
{
    public static class Helpers
    {
        public static void CursorLock(bool isLocked)
        {
            Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !isLocked;
        }

        public static void IsPauseTime(bool isPaused) => Time.timeScale = isPaused ? 0f : 1f;

        public static bool IsInLayerMask(GameObject obj, LayerMask layerMask) => (layerMask.value & (1 << obj.layer)) != 0;
    }
}