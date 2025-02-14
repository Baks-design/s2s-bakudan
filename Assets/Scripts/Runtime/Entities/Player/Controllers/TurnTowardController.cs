using KBCore.Refs;
using UnityEngine;

namespace Game.Runtime.Entities.Player.Controllers
{
    public class TurnTowardController : MonoBehaviour
    {
        [SerializeField, Self] Transform tr;
        [SerializeField, Parent] MovementController movementController;
        [SerializeField] float turnSpeed = 50f;
        float currentYRotation;
        const float FallOffAngle = 90f;

        void Start() => currentYRotation = tr.localEulerAngles.y;

        void LateUpdate()
        {
            if (movementController == null || tr == null) return;

            var velocity = Vector3.ProjectOnPlane(movementController.GetMovementVelocity, tr.parent.up);

            var velocityMagnitude = velocity.magnitude;
            if (velocityMagnitude < 0.001f) return;

            var targetAngle = Vector3.SignedAngle(tr.forward, velocity.normalized, tr.parent.up);

            var rotationStep = Mathf.MoveTowards(
                0f, targetAngle, turnSpeed * Time.deltaTime * Mathf.InverseLerp(0f, FallOffAngle, Mathf.Abs(targetAngle)));
            currentYRotation += rotationStep;

            tr.localRotation = Quaternion.Euler(0f, currentYRotation, 0f);
        }
    }
}
