using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static Game.Runtime.Components.PlayerInputActions;

namespace Game.Runtime.Components.Inputs
{
    [CreateAssetMenu(menuName = "Data/System/InputReader")]
    public class InputReader : ScriptableObject, IPlayerActions, IUIActions
    {
        public PlayerInputActions inputActions;

        public Vector2 Direction => inputActions.Player.Move.ReadValue<Vector2>();
        public bool IsPlayerMapActive { get; set; }
        public bool IsUIMapActive { get; set; }

        public event Action OpenMenu = delegate { };
        public event Action<Vector2> Move = delegate { };
        public event Action<Vector2, bool> Look = delegate { };
        public event Action<bool> Jump = delegate { };
        public event Action CloseMenu = delegate { };

        void OnEnable() => SetupMaps();

        void OnDisable() => DisableMaps();

        void SetupMaps()
        {
            if (inputActions == null)
            {
                inputActions = new PlayerInputActions();
                inputActions.Player.SetCallbacks(this);
                inputActions.UI.SetCallbacks(this);
            }
        }

        public void EnablePlayerMap()
        {
            inputActions.Player.Enable();
            inputActions.UI.Disable();
            IsPlayerMapActive = true;
            IsUIMapActive = false;
        }

        public void EnableUIMap()
        {
            inputActions.Player.Disable();
            inputActions.UI.Enable();
            IsPlayerMapActive = false;
            IsUIMapActive = true;
        }

        public void DisableMaps()
        {
            inputActions.Player.Disable();
            inputActions.UI.Disable();
            IsPlayerMapActive = false;
            IsUIMapActive = false;
        }

        public void OnOpenMenu(InputAction.CallbackContext context)
        {
            if (context.phase is InputActionPhase.Started)
                OpenMenu.Invoke();
        }

        public void OnMove(InputAction.CallbackContext context) => Move.Invoke(context.ReadValue<Vector2>());

        public void OnLook(InputAction.CallbackContext context) => Look.Invoke(context.ReadValue<Vector2>(), IsDeviceMouse(context));

        bool IsDeviceMouse(InputAction.CallbackContext context) => context.control.device.name.Equals("Keyboard&Mouse");

        public void OnJump(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    Jump.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    Jump.Invoke(false);
                    break;
            }
        }

        public void OnClick(InputAction.CallbackContext context)
        {
        }

        public void OnRightClick(InputAction.CallbackContext context)
        {
        }

        public void OnMiddleClick(InputAction.CallbackContext context)
        {
        }

        public void OnScrollWheel(InputAction.CallbackContext context)
        {
        }

        public void OnPoint(InputAction.CallbackContext context)
        {
        }

        public void OnNavigate(InputAction.CallbackContext context)
        {
        }

        public void OnSubmit(InputAction.CallbackContext context)
        {
        }

        public void OnCancel(InputAction.CallbackContext context)
        {
        }

        public void OnCloseMenu(InputAction.CallbackContext context)
        {
            if (context.phase is InputActionPhase.Started)
                CloseMenu.Invoke();
        }
    }
}