using Game.Runtime.Utilities.Patterns.EventBus;

namespace Game.Runtime.Components.Events
{
    public struct GameStateEvent : IEvent
    {
        public enum GameState
        {
            Gameplay,
            Menu
        }
        public GameState CurrentGameState;
    }
}