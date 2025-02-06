using System;
using UnityEngine;

namespace Game.Runtime.Components.Damage
{
    public interface IDamageable
    {
        int CurrentHealth { get; }
        bool Invincible { get; }
        bool IsAbleToCure { get; }

        event Action<int, GameObject> OnDamaged;
        event Action OnDeath;

        void TakeDamage(int damage, GameObject damageSource);
        void Kill();
    }
}