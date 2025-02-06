using Game.Runtime.Utilities.Helpers;
using Game.Runtime.Utilities.Patterns.StateMachines;

namespace Game.Runtime.Systems.Management.States
{
    public class PauseState : IState
    {
        readonly GameManager _manager;

        public PauseState(GameManager manager) => _manager = manager;

        public void OnEnter()
        {
            Helpers.SetCursorLock(false);
            _manager.InputReader.ChangeToUIMap();
        }
    }
}