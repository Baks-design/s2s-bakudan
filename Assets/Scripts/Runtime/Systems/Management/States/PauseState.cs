using Game.Runtime.Utilities.Helpers;
using Game.Runtime.Utilities.Patterns.StateMachines;

namespace Game.Runtime.Systems.Management.States
{
    public class PauseState : IState
    {
        public PauseState(GameManager manager) { }

        public void OnEnter()
        {
            Helpers.CursorLock(false);
            Helpers.IsPauseTime(true);
        }

        public void Update() => Helpers.CursorLock(false);
    }
}