using Game.Runtime.Entities.Player.Controllers;
using Game.Runtime.Utilities.Patterns.StateMachines;

namespace Game.Runtime.Entities.Player.States
{
    public class PlayerJumpingState : IState
    {
        readonly PlayerMovementController controller;

        public PlayerJumpingState(PlayerMovementController controller) => this.controller = controller;

        public void OnEnter()
        {
            controller.OnGroundContactLost();
            controller.OnJumpStart();
        }

        public void FixedUpdate() => controller.HandleJumping();
    }
}