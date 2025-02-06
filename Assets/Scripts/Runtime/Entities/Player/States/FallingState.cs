using Game.Runtime.Entities.Player.Controllers;
using Game.Runtime.Utilities.Patterns.StateMachines;

namespace Game.Runtime.Entities.Player.States
{
    public class FallingState : IState
    {
        readonly MovementController controller;

        public FallingState(MovementController controller) => this.controller = controller;

        public void OnEnter() => controller.OnFallStart();
    }
}