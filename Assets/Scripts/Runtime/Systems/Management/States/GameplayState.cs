using Game.Runtime.Utilities.Helpers;
using Game.Runtime.Utilities.Patterns.StateMachines;

namespace Game.Runtime.Systems.Management.States
{
    public class GameplayState : IState
    {
        readonly GameManager manager;

        public GameplayState(GameManager manager) => this.manager = manager;

        public void Update() => Helpers.LockCursor();
    }
}