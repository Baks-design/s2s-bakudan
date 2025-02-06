using System;
using Game.Runtime.Systems.Management;
using UnityEngine;
using UnityEngine.InputSystem;
using static Game.Runtime.Systems.Management.PlayerInputActions;

namespace Game.Runtime.Systems.Inputs
{
    [CreateAssetMenu(menuName = "Data/System/InputReader")]
    public class InputReader : ScriptableObject, IPlayerActions, IInputReader
    {
        public PlayerInputActions inputActions;

        public Vector2 Direction => inputActions.Player.Move.ReadValue<Vector2>();
        public Vector2 LookDirection => inputActions.Player.Look.ReadValue<Vector2>();

        public event Action<bool> OpenMenu = delegate { };
        public event Action<Vector2> Move = delegate { };
        public event Action<Vector2, bool> Look = delegate { };
        public event Action<bool> Throw = delegate { };

        public void EnablePlayerActions()
        {
            if (inputActions == null)
            {
                inputActions = new PlayerInputActions();
                inputActions.Player.SetCallbacks(this);
            }
            inputActions.Enable();
        }

        public void ChangeToPlayerMap()
        {
            inputActions.Player.Enable();
            inputActions.UI.Disable();
        }

        public void ChangeToUIMap()
        {
            inputActions.Player.Disable();
            inputActions.UI.Enable();
        }

        public void OnOpenMenu(InputAction.CallbackContext context)
        {
            if (context.phase is InputActionPhase.Started)
                OpenMenu.Invoke(true);
        }

        public void OnMove(InputAction.CallbackContext context) => Move.Invoke(context.ReadValue<Vector2>());

        public void OnLook(InputAction.CallbackContext context) => Look.Invoke(context.ReadValue<Vector2>(), IsDeviceMouse(context));

        bool IsDeviceMouse(InputAction.CallbackContext context) => context.control.device.name.Equals("Keyboard&Mouse");

        public void OnThrow(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    Throw.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    Throw.Invoke(false);
                    break;
            }
        }
    }
}