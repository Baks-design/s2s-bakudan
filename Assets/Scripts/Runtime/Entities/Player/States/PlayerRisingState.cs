using Game.Runtime.Entities.Player.Controllers;
using Game.Runtime.Utilities.Patterns.StateMachines;

namespace Game.Runtime.Entities.Player.States
{
    public class PlayerRisingState : IState
    {
        readonly PlayerMovementController controller;

        public PlayerRisingState(PlayerMovementController controller) => this.controller = controller;

        public void OnEnter() => controller.OnGroundContactLost();
    }
}