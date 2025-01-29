using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Cinemachine;

namespace Game.Runtime.Entities.Player.Controllers
{
    public abstract class ActionsControllerBase : MonoBehaviour, IInputAxisOwner
    {
        public float ThrowForce = 1f;

        [Header("Input Axes")]
        public InputAxis Throw = InputAxis.DefaultMomentary;

        public bool IsThrow => Throw.Value > 0.01f;

        public Action OnThrow = delegate { };

        /// <summary>
        /// Report the available input axes to the input axis controller.
        /// This supports both the new Input System and the legacy input system.
        /// </summary>
        void IInputAxisOwner.GetInputAxes(List<IInputAxisOwner.AxisDescriptor> axes)
        {
            axes.Add(new IInputAxisOwner.AxisDescriptor
            {
                DrivenAxis = () => ref Throw,
                Name = "Throw"
            });
        }
    }
}