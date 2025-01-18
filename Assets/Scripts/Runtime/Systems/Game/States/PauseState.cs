using Game.Runtime.Utilities.Patterns.StateMachines;

namespace Game.Runtime.Systems.GameManagement.States
{
    public class PauseState : IState
    {
        readonly GameManager _manager;

        public PauseState(GameManager manager) => _manager = manager;

        public void OnEnter()
        {
            GameManager.SetCursorLock(false);
            _manager.InputReader.ChangeToUIMap();
        }
    }
}