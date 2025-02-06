using UnityEngine;

namespace Game.Runtime.Utilities.Extensions
{
    using UnityEngine;

    public static class UIExtensions
    {
        /// <summary>
        /// Extracts the position from a 4x4 transformation matrix.
        /// </summary>
        public static Vector3 GetPosition(this Matrix4x4 matrix) => matrix.GetColumn(3);

        /// <summary>
        /// Extracts the rotation from a 4x4 transformation matrix.
        /// </summary>
        public static Quaternion GetRotation(this Matrix4x4 matrix)
        {
            var forward = matrix.GetColumn(2);
            var upwards = matrix.GetColumn(1);
            return Quaternion.LookRotation(forward, upwards);
        }

        /// <summary>
        /// Converts a Vector3 to a Vector2 by discarding the Y component.
        /// </summary>
        public static Vector2 XZ(this Vector3 vector) => new(vector.x, vector.z);
    }
}