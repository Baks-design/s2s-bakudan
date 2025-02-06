using KBCore.Refs;
using UnityEngine;

namespace Game.Runtime.Utilities.Helpers
{
    /// <summary>
    /// Orients the GameObject that this script is attached in such a way that it always faces the Target.
    /// </summary>
    [ExecuteAlways]
    public class LookAtTarget : MonoBehaviour
    {
        [SerializeField, Self] Transform tr;
        [SerializeField] Transform Target;
        [SerializeField] bool LockRotationX;
        [SerializeField] bool LockRotationY;
        [SerializeField] bool LockRotationZ;
        Vector3 m_InitialRotation;

        void Awake() => m_InitialRotation = tr.rotation.eulerAngles;

        void Reset() => m_InitialRotation = tr.rotation.eulerAngles;

        void LateUpdate()
        {
            var direction = Target.position - tr.position;
            tr.rotation = Quaternion.LookRotation(direction);

            if (LockRotationX || LockRotationY || LockRotationZ)
            {
                var euler = tr.rotation.eulerAngles;
                if (LockRotationX) euler.x = m_InitialRotation.x;
                if (LockRotationY) euler.y = m_InitialRotation.y;
                if (LockRotationZ) euler.z = m_InitialRotation.z;
                tr.rotation = Quaternion.Euler(euler);
            }
        }
    }
}
