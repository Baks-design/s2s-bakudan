using Game.Runtime.Components.Events;
using Game.Runtime.Utilities.Patterns.EventBus;

namespace Game.Runtime.Components.Damage
{
    public class PlayerHealth : Damageable
    {
        protected override void Start() => base.Start();

        void Update() => HandleHealth();

        void HandleHealth() => EventBus<PlayerHealthEvent>.Raise(new PlayerHealthEvent { currentHealth = CurrentHealth });
    }
}