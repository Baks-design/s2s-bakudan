using UnityEngine;

namespace Game.Runtime.Systems.GameManagement
{
    [CreateAssetMenu(menuName = "Data/System/InputReader")]
    public class InputReader : ScriptableObject, IInputReader
    {
        public PlayerInputActions inputActions;

        public void EnableInputActions() => inputActions ??= new PlayerInputActions();

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
    }
}