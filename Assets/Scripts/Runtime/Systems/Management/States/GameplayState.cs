using Game.Runtime.Utilities.Helpers;
using Game.Runtime.Utilities.Patterns.StateMachines;

namespace Game.Runtime.Systems.Management.States
{
    public class GameplayState : IState
    {
        readonly GameManager _manager;

        public GameplayState(GameManager manager) => _manager = manager;

        public void OnEnter()
        {
            Helpers.SetCursorLock(true);
            _manager.InputReader.ChangeToPlayerMap();
        }
    }
}