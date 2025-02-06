using Game.Runtime.Entities.Player.Controllers;
using Game.Runtime.Utilities.Patterns.StateMachines;

namespace Game.Runtime.Entities.Player.States
{
    public class RisingState : IState
    {
        readonly MovementController controller;

        public RisingState(MovementController controller) => this.controller = controller;

        public void OnEnter() => controller.OnGroundContactLost();
    }
}