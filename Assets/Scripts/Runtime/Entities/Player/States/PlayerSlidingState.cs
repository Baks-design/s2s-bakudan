using Game.Runtime.Entities.Player.Controllers;
using Game.Runtime.Utilities.Patterns.StateMachines;

namespace Game.Runtime.Entities.Player.States
{
    public class PlayerSlidingState : IState
    {
        readonly PlayerMovementController controller;

        public PlayerSlidingState(PlayerMovementController controller) => this.controller = controller;

        public void OnEnter() => controller.OnGroundContactLost();

        public void FixedUpdate() => controller.SlidingMomentum();
    }
}