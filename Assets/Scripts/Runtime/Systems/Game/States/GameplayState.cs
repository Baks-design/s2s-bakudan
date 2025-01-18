using Game.Runtime.Utilities.Patterns.StateMachines;

namespace Game.Runtime.Systems.GameManagement.States
{
    public class GameplayState : IState
    {
        readonly GameManager _manager;

        public GameplayState(GameManager manager) => _manager = manager;

        public void OnEnter()
        {
            GameManager.SetCursorLock(true);
            _manager.InputReader.ChangeToPlayerMap();
        }
    }
}