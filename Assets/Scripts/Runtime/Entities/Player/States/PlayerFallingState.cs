using Game.Runtime.Entities.Player.Controllers;
using Game.Runtime.Utilities.Patterns.StateMachines;

namespace Game.Runtime.Entities.Player.States
{
    public class PlayerFallingState : IState
    {
        readonly PlayerMovementController controller;

        public PlayerFallingState(PlayerMovementController controller) => this.controller = controller;

        public void OnEnter() => controller.OnFallStart();
    }
}