using System.Collections.Generic;
using KBCore.Refs;
using Unity.Cinemachine;
using UnityEngine;

namespace Game.Runtime.Entities.Player.Controllers
{
    /// <summary>
    /// This is an add-on for SimplePlayerController that controls the player's Aiming Core.
    /// 
    /// This component expects to be in a child object of a player that has a SimplePlayerController 
    /// behaviour.  It works intimately with that component.
    //
    /// The purpose of the aiming core is to decouple the camera rotation from the player rotation.  
    /// Camera rotation is determined by the rotation of the player core GameObject, and this behaviour 
    /// provides input axes for controlling it.  When the player core is used as the target for 
    /// a CinemachineCamera with a ThirdPersonFollow component, the camera will look along the core's 
    /// forward axis, and pivot around the core's origin.
    /// 
    /// The aiming core is also used to define the origin and direction of player shooting, if player 
    /// has that ability.  
    /// 
    /// To implement player shooting, add a SimplePlayerShoot behaviour to this GameObject.
    /// </summary>
    public class AimController : MonoBehaviour, IInputAxisOwner
    {
        public enum CouplingMode { Coupled, CoupledWhenMoving, Decoupled }

        [SerializeField] Transform aimTransform;
        [SerializeField] MovementControllerBase movementController;
        [SerializeField] CouplingMode playerRotation = CouplingMode.Coupled;
        [SerializeField] float rotationDamping = 0.2f;
        [SerializeField]
        InputAxis horizontalLook = new()
        {
            Range = new Vector2(-180f, 180f),
            Wrap = true,
            Recentering = InputAxis.RecenteringSettings.Default
        };
        [SerializeField]
        InputAxis verticalLook = new()
        {
            Range = new Vector2(-70f, 70f),
            Recentering = InputAxis.RecenteringSettings.Default
        };
        Transform controllerTransform;
        Quaternion desiredWorldRotation;

        public CouplingMode PlayerRotation => playerRotation;
        public float RotationDamping => rotationDamping;

        void IInputAxisOwner.GetInputAxes(List<IInputAxisOwner.AxisDescriptor> axes)
        {
            axes.Add(new()
            {
                DrivenAxis = () => ref horizontalLook,
                Name = "Horizontal Look",
                Hint = IInputAxisOwner.AxisDescriptor.Hints.X
            });
            axes.Add(new()
            {
                DrivenAxis = () => ref verticalLook,
                Name = "Vertical Look",
                Hint = IInputAxisOwner.AxisDescriptor.Hints.Y
            });
        }

        void OnValidate()
        {
            horizontalLook.Validate();
            verticalLook.Range = new Vector2(
                Mathf.Clamp(verticalLook.Range.x, -90f, 90f),
                Mathf.Clamp(verticalLook.Range.y, -90f, 90f)
            );
            verticalLook.Validate();
        }

        void OnEnable()
        {
            movementController.PreUpdate += UpdatePlayerRotation;
            movementController.PostUpdate += PostUpdate;

            controllerTransform = movementController.transform;
        }

        void OnDisable()
        {
            movementController.PreUpdate -= UpdatePlayerRotation;
            movementController.PostUpdate -= PostUpdate;

            controllerTransform = null;
        }

        public void RecenterPlayer(float damping = 0f)
        {
            if (controllerTransform == null) return;

            var localRotation = aimTransform.localRotation.eulerAngles;
            localRotation.y = NormalizeAngle(localRotation.y);
            var deltaY = Damper.Damp(localRotation.y, damping, Time.deltaTime);

            controllerTransform.rotation = 
                Quaternion.AngleAxis(deltaY, controllerTransform.up) * controllerTransform.rotation;
            horizontalLook.Value -= deltaY;
            localRotation.y -= deltaY;
            aimTransform.localRotation = Quaternion.Euler(localRotation);
        }

        public void SetLookDirection(Vector3 worldspaceDirection)
        {
            if (controllerTransform == null) return;

            var localRotation = Quaternion.Inverse(controllerTransform.rotation) *
                                Quaternion.LookRotation(worldspaceDirection, controllerTransform.up);
            var eulerAngles = localRotation.eulerAngles;

            horizontalLook.Value = horizontalLook.ClampValue(eulerAngles.y);
            verticalLook.Value = verticalLook.ClampValue(NormalizeAngle(eulerAngles.x));
        }

        void UpdatePlayerRotation()
        {
            aimTransform.localRotation = Quaternion.Euler(verticalLook.Value, horizontalLook.Value, 0f);
            desiredWorldRotation = aimTransform.rotation;

            switch (playerRotation)
            {
                case CouplingMode.Coupled:
                    movementController.SetStrafeMode(true);
                    RecenterPlayer();
                    break;

                case CouplingMode.CoupledWhenMoving:
                    movementController.SetStrafeMode(true);
                    if (movementController.IsMoving)
                        RecenterPlayer(rotationDamping);
                    break;

                case CouplingMode.Decoupled:
                    movementController.SetStrafeMode(false);
                    break;
            }

            verticalLook.UpdateRecentering(Time.deltaTime, verticalLook.TrackValueChange());
            horizontalLook.UpdateRecentering(Time.deltaTime, horizontalLook.TrackValueChange());
        }

        void PostUpdate(Vector3 velocity, float speed)
        {
            if (playerRotation == CouplingMode.Decoupled)
            {
                aimTransform.rotation = desiredWorldRotation;

                var deltaRotation = Quaternion.Inverse(controllerTransform.rotation) * desiredWorldRotation;
                var eulerAngles = deltaRotation.eulerAngles;

                verticalLook.Value = NormalizeAngle(eulerAngles.x);
                horizontalLook.Value = NormalizeAngle(eulerAngles.y);
            }
        }

        float NormalizeAngle(float angle)
        {
            while (angle > 180f) angle -= 360f;
            while (angle < -180f) angle += 360f;
            return angle;
        }
    }
}