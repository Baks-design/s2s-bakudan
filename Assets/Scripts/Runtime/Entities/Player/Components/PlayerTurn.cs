using KBCore.Refs;
using UnityEngine;
using Game.Runtime.Entities.Player.Controllers;
using Game.Runtime.Utilities.Helpers;

namespace Game.Runtime.Entities.Player.Components
{
    public class TurnTowardController : MonoBehaviour
    {
        [SerializeField, Self] Transform tr;
        [SerializeField, Parent] PlayerMovementController controller;
        [SerializeField] float turnSpeed = 500f;
        float currentYRotation;

        void Start() => currentYRotation = tr.localEulerAngles.y;

        void LateUpdate()
        {
            var velocity = Vector3.ProjectOnPlane(controller.GetMovementVelocity, tr.parent.up);
            if (velocity.magnitude < 0.001f)
                return;

            var angleDifference = VectorMath.GetAngle(tr.forward, velocity.normalized, tr.parent.up);

            var step = Mathf.Sign(angleDifference) *
                        Mathf.InverseLerp(0f, 90f, Mathf.Abs(angleDifference)) *
                        Time.deltaTime * turnSpeed;

            currentYRotation += Mathf.Abs(step) > Mathf.Abs(angleDifference) ? angleDifference : step;

            tr.localRotation = Quaternion.Euler(0f, currentYRotation, 0f);
        }
    }
}
