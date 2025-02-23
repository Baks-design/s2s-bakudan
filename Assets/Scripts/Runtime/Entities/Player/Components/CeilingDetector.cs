using UnityEngine;

namespace Game.Runtime.Entities.Player.Components
{
    public class CeilingDetector : MonoBehaviour
    {
        [SerializeField] float ceilingAngleLimit = 10f;
        [SerializeField] bool isInDebugMode = false;

        public bool HitCeiling { get; private set; }

        void OnCollisionEnter(Collision collision) => CheckForContact(collision);

        void OnCollisionStay(Collision collision) => CheckForContact(collision);

        void CheckForContact(Collision collision)
        {
            if (collision.contacts.Length == 0)
                return;

            var angle = Vector3.Angle(-transform.up, collision.contacts[0].normal);
            if (angle < ceilingAngleLimit)
                HitCeiling = true;

            if (isInDebugMode)
                Debug.DrawRay(collision.contacts[0].point, collision.contacts[0].normal, Color.red, 2f);
        }

        public void Reset() => HitCeiling = false;
    }
}

