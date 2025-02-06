using KBCore.Refs;
using UnityEngine;
using System;

namespace Game.Runtime.Entities.Player.Components
{
    public class CeilingDetector : MonoBehaviour
    {
        [SerializeField, Self] Transform tr;
        [SerializeField] LayerMask ceilingLayerMask;
        [SerializeField] float ceilingAngleLimit = 10f;
        [SerializeField] bool isInDebugMode = false;
        bool _ceilingWasHit;
        const float DebugDrawDuration = 2f;

        public bool HitCeiling => _ceilingWasHit;

        public event Action OnCeilingHit = delegate { };

        void OnCollisionEnter(Collision collision) => CheckForContact(collision);

        void OnCollisionStay(Collision collision) => CheckForContact(collision);

        void OnDrawGizmos()
        {
            if (!isInDebugMode || tr == null) return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(tr.position, -tr.up * 2f);

            Gizmos.color = Color.cyan;
            Quaternion leftRotation = Quaternion.AngleAxis(-ceilingAngleLimit, tr.right);
            Quaternion rightRotation = Quaternion.AngleAxis(ceilingAngleLimit, tr.right);
            Vector3 leftDirection = leftRotation * -tr.up;
            Vector3 rightDirection = rightRotation * -tr.up;
            Gizmos.DrawRay(tr.position, leftDirection * 2f);
            Gizmos.DrawRay(tr.position, rightDirection * 2f);
        }

        void CheckForContact(Collision collision)
        {
            if ((ceilingLayerMask.value & (1 << collision.gameObject.layer)) == 0) return;

            for (var i = 0; i < collision.contactCount; i++)
            {
                var contact = collision.GetContact(i);
                var angle = Vector3.Angle(-tr.up, contact.normal); 

                if (angle < ceilingAngleLimit)
                {
                    _ceilingWasHit = true; 
                    OnCeilingHit?.Invoke(); 
                    break; 
                }

                if (isInDebugMode)
                    DebugDrawContact(contact);
            }
        }

        void DebugDrawContact(ContactPoint contact) => Debug.DrawRay(contact.point, contact.normal, Color.red, DebugDrawDuration);

        public void ResetCeilingDetection() => _ceilingWasHit = false; 
    }
}

