using Game.Runtime.Entities.Player.Controllers;
using Game.Runtime.Utilities.Patterns.StateMachines;

namespace Game.Runtime.Entities.Player.States
{
    public class GroundedState : IState
    {
        readonly MovementController controller;

        public GroundedState(MovementController controller) => this.controller = controller;

        public void OnEnter() => controller.OnGroundContactRegained();
    }
}