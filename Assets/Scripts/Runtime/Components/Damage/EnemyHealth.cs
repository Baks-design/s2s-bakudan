using System;
using UnityEngine;

namespace Game.Runtime.Components.Damage
{
    public class EnemyHealth : Health
    {
        public event Action OnEnemyDeath = delegate { };

        protected override void Start() => base.Start();

        public override void Heal(float healAmount) => Debug.Log("Enemy cannot be healed.");

        public override void TakeDamage(float damage, GameObject damageSource) => base.TakeDamage(damage, damageSource);

        protected override void HandleDeath()
        {
            base.HandleDeath();
            OnEnemyDeath.Invoke();
            Debug.Log("Enemy has died and dropped loot!");
        }
    }
}