using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Cinemachine;

namespace Game.Runtime.Entities.Player.Controllers
{
    /// <summary>
    /// **Services:**
    /// 
    ///  - 2D motion axes (MoveX and MoveZ)
    ///  - Jump button
    ///  - Sprint button
    ///  - API for strafe mode
    /// 
    /// **Actions:**
    /// 
    ///  - PreUpdate - invoked at the beginning of `Update()`
    ///  - PostUpdate - invoked at the end of `Update()`
    ///  - StartJump - invoked when the player starts jumping
    ///  - EndJump - invoked when the player stops jumping
    /// 
    /// **Events:** 
    /// 
    ///  - Landed - invoked when the player lands on the ground
    /// </summary>
    public abstract class MovementControllerBase : MonoBehaviour, IInputAxisOwner
    {
        public float Speed = 1f;
        public float SprintSpeed = 4f;
        public float JumpSpeed = 4f;
        public float SprintJumpSpeed = 6f;

        [Header("Input Axes")]
        public InputAxis MoveX = InputAxis.DefaultMomentary;
        public InputAxis MoveZ = InputAxis.DefaultMomentary;
        public InputAxis Jump = InputAxis.DefaultMomentary;
        public InputAxis Sprint = InputAxis.DefaultMomentary;

        public Action Landed = delegate { };
        public Action PreUpdate = delegate { };
        public Action<Vector3, float> PostUpdate = delegate { };
        public Action StartJump = delegate { };
        public Action EndJump = delegate { };

        /// <summary>
        /// Report the available input axes to the input axis controller.
        /// This supports both the new Input System and the legacy input system.
        /// </summary>
        void IInputAxisOwner.GetInputAxes(List<IInputAxisOwner.AxisDescriptor> axes)
        {
            axes.Add(new IInputAxisOwner.AxisDescriptor
            {
                DrivenAxis = () => ref MoveX,
                Name = "Move X",
                Hint = IInputAxisOwner.AxisDescriptor.Hints.X
            });
            axes.Add(new IInputAxisOwner.AxisDescriptor
            {
                DrivenAxis = () => ref MoveZ,
                Name = "Move Z",
                Hint = IInputAxisOwner.AxisDescriptor.Hints.Y
            });
            axes.Add(new IInputAxisOwner.AxisDescriptor
            {
                DrivenAxis = () => ref Jump,
                Name = "Jump"
            });
            axes.Add(new IInputAxisOwner.AxisDescriptor
            {
                DrivenAxis = () => ref Sprint,
                Name = "Sprint"
            });
        }

        /// <summary>
        /// Sets the strafe mode for movement.
        /// </summary>
        /// <param name="b">If true, enables strafe mode; otherwise, disables it.</param>
        public virtual void SetStrafeMode(bool b) { }

        /// <summary>
        /// Determines if the character is currently moving.
        /// </summary>
        public abstract bool IsMoving { get; }
    }
}