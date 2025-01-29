using UnityEngine;

namespace Game.Runtime.Utilities.Extensions
{
    public static class UIExtensions
    {
        public static Vector3 GetPosition(this Matrix4x4 m) => new(m.m03, m.m13, m.m23);

        public static Quaternion GetRotation(this Matrix4x4 m)
        {
            var forward = new Vector3(m.m02, m.m12, m.m22);
            var upwards = new Vector3(m.m01, m.m11, m.m21);
            return Quaternion.LookRotation(forward, upwards);
        }

        public static Vector2 XZ(this Vector3 v) => new(v.x, v.z);
    }
}