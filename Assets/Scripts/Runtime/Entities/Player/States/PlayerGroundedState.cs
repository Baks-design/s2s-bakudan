using Game.Runtime.Entities.Player.Controllers;
using Game.Runtime.Utilities.Patterns.StateMachines;

namespace Game.Runtime.Entities.Player.States
{
    public class PlayerGroundedState : IState
    {
        readonly PlayerMovementController controller;

        public PlayerGroundedState(PlayerMovementController controller) => this.controller = controller;

        public void OnEnter() => controller.OnGroundContactRegained();

        public void FixedUpdate() => controller.GroundMomentum();
    }
}