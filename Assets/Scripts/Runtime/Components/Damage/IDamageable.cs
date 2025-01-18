using System;
using UnityEngine;

namespace Game.Runtime.Components.Damage
{
    public interface IDamageable
    {
        float CurrentHealth { get; }
        bool Invincible { get; set; }
        float GetRatio { get; }
        bool IsCritical { get; }

        event Action<float, GameObject> OnDamaged;
        event Action OnDie;

        void TakeDamage(float damage, GameObject damageSource);
        void Kill();
    }
}