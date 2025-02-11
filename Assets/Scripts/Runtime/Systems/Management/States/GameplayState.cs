using Game.Runtime.Utilities.Helpers;
using Game.Runtime.Utilities.Patterns.StateMachines;

namespace Game.Runtime.Systems.Management.States
{
    public class GameplayState : IState
    {
        public GameplayState(GameManager manager) { }

        public void OnEnter()
        {
            Helpers.CursorLock(true);
            Helpers.IsPauseTime(false);
        }

        public void Update() => Helpers.CursorLock(true);
    }
}