using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game.Runtime.Utilities.Helpers
{
    public static class Helpers
    {
        public static WaitForSeconds GetWaitForSeconds(float seconds) => WaitFor.Seconds(seconds);

        /// <summary>
        /// Clears the console log in the Unity Editor.
        /// </summary>
#if UNITY_EDITOR
        public static void ClearConsole()
        {
            var assembly = Assembly.GetAssembly(typeof(SceneView));
            var type = assembly.GetType("UnityEditor.LogEntries");
            var method = type.GetMethod("Clear");
            method?.Invoke(new object(), null);
        }
#endif      

        public static void SetCursorLock(bool isLocked)
        {
            Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !isLocked;
        }

        public static bool IsInLayerMask(GameObject obj, LayerMask layerMask) => (layerMask.value & (1 << obj.layer)) != 0;

        public static float ExponentialEase(float t, float power) => Mathf.Pow(t, power);
    }
}