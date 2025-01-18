using KBCore.Refs;
using UnityEngine;

namespace Game.Runtime.Entities.Player.Components
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
        Vector3 m_Rotation;

        void OnEnable() => m_Rotation = transform.rotation.eulerAngles;

        void Reset() => m_Rotation = transform.rotation.eulerAngles;

        void Update()
        {
            if (Target == null) return;

            var direction = Target.position - tr.position;
            tr.rotation = Quaternion.LookRotation(direction);

            if (LockRotationX || LockRotationY || LockRotationZ)
            {
                var euler = tr.rotation.eulerAngles;
                euler.x = LockRotationX ? m_Rotation.x : euler.x;
                euler.y = LockRotationY ? m_Rotation.y : euler.y;
                euler.z = LockRotationZ ? m_Rotation.z : euler.z;
                tr.rotation = Quaternion.Euler(euler);
            }
        }
    }
}
