using Game.Runtime.Systems.Interaction;
using KBCore.Refs;
using UnityEngine;

namespace Game.Runtime.Entities.Player.Controllers
{
    public class ForcesController : MonoBehaviour, IImpactable
    {
        [SerializeField, Self] CharacterController cc;
        [SerializeField] float pushForce = 5f;

        public void ApplyForce(Vector3 throwDirection, float throwForce)
        {
           
        }

        public void ApplyForceBy(float applyForce, float applyRadius, Vector3 center)
        {
            var direction = cc.transform.position - center;
            var distance = direction.magnitude;
            if (distance > applyRadius) return;

            direction.Normalize();

            var force = applyForce * (1f - (distance / applyRadius));

            var velocity = force * Time.deltaTime * direction;

            cc.Move(velocity);
        }

        void OnControllerColliderHit(ControllerColliderHit hit) => PushRigidBodies(hit);

        void PushRigidBodies(ControllerColliderHit hit)
        {
            var hitRigidbody = hit.collider.attachedRigidbody;
            if (hitRigidbody != null && !hitRigidbody.isKinematic)
            {
                var pushDirection = hit.moveDirection;
                hitRigidbody.AddForce(pushDirection * pushForce, ForceMode.Impulse);
            }
        }
    }
}