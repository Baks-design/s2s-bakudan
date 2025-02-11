using Game.Runtime.Utilities.Patterns.EventBus;

namespace Game.Runtime.Components.Events
{
    public struct PlayerEvent : IEvent
    {
        public float Health;
    }
}