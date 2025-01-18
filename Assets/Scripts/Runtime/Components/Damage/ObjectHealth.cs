using System;
using UnityEngine;

namespace Game.Runtime.Components.Damage
{
    public class ObjectHealth : Health
    {
        public event Action OnObjectDestroyed = delegate { };

        protected override void Start() => base.Start();

        public override void TakeDamage(float damage, GameObject damageSource) => base.TakeDamage(damage, damageSource);

        protected override void HandleDeath()
        {
            base.HandleDeath();
            OnObjectDestroyed.Invoke();
            Debug.Log("Object has destroyed");
        }
    }
}