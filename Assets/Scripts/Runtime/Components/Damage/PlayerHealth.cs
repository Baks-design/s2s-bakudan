using System;
using UnityEngine;

namespace Game.Runtime.Components.Damage
{
    public class PlayerHealth : Health
    {
        public event Action OnPlayerCriticalHealth = delegate { };

        protected override void Start() => base.Start();

        public override void Heal(float healAmount) => base.Heal(healAmount);

        public override void TakeDamage(float damage, GameObject damageSource)
        {
            base.TakeDamage(damage, damageSource);
            if (IsCritical)
                OnPlayerCriticalHealth.Invoke();
        }

        protected override void HandleDeath()
        {
            base.HandleDeath();
            Debug.Log("Player has died!");
        }
    }
}