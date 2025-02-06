using Game.Runtime.Entities.Player.Controllers;
using Game.Runtime.Utilities.Patterns.StateMachines;

namespace Game.Runtime.Entities.Player.States
{
    public class SlidingState : IState
    {
        readonly MovementController controller;

        public SlidingState(MovementController controller) => this.controller = controller;

        public void OnEnter() => controller.OnGroundContactLost();
    }
}