using Game.Runtime.Utilities.Helpers;
using Game.Runtime.Utilities.Patterns.StateMachines;

namespace Game.Runtime.Systems.Management.States
{
    public class PauseState : IState
    {
        readonly GameManager manager;

        public PauseState(GameManager manager) => this.manager = manager;

        public void Update() => Helpers.UnlockCursor();
    }
}