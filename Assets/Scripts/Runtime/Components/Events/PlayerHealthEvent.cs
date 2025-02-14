using Game.Runtime.Utilities.Patterns.EventBus;

namespace Game.Runtime.Components.Events
{
    public struct PlayerHealthEvent : IEvent
    {
        public float currentHealth;
    }
}