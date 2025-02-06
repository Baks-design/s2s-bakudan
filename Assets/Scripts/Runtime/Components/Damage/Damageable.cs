using System;
using UnityEngine;

namespace Game.Runtime.Components.Damage
{
    public class Damageable : MonoBehaviour, IDamageable, IHealable
    {
        [SerializeField, Range(1, 100)] int maxHealth = 100;
        bool isDead;

        public int CurrentHealth { get; private set; }
        public bool Invincible { get; private set; }
        public bool IsDead => isDead;
        public bool IsAbleToCure => maxHealth > 0 && ((float)CurrentHealth / maxHealth) <= 0.75f;

        public event Action OnDeath = delegate { };
        public event Action<int, GameObject> OnDamaged = delegate { };
        public event Action<int> OnHealed = delegate { };

        void Start() => Reset();

        public void Reset()
        {
            CurrentHealth = maxHealth;
            isDead = false;
            Invincible = false;
        }

        public void SetInvincible(bool invincible) => Invincible = invincible;

        public void Heal(int healAmount)
        {
            if (isDead || healAmount <= 0) return;

            var trueHealAmount = ApplyHealing(healAmount);
            if (trueHealAmount > 0)
            {
                OnHealed?.Invoke(trueHealAmount);
                Debug.Log($"Healed by {trueHealAmount}. Current Health: {CurrentHealth}");
            }
        }

        public void TakeDamage(int damage, GameObject damageSource)
        {
            if (Invincible || isDead || damage <= 0) return;

            var trueDamageAmount = ApplyDamage(damage);
            if (trueDamageAmount > 0)
            {
                OnDamaged?.Invoke(trueDamageAmount, damageSource);
                Debug.Log($"Took {trueDamageAmount} damage from {damageSource.name}. Current Health: {CurrentHealth}");
            }

            if (CurrentHealth <= 0 && !isDead)
                HandleDeath();
        }

        public void Kill()
        {
            if (isDead) return;

            CurrentHealth = 0;
            HandleDeath();
        }

        void HandleDeath()
        {
            isDead = true;
            OnDeath?.Invoke();
            Debug.Log($"Entity {gameObject} has died!");
        }

        int ApplyHealing(int healAmount)
        {
            var healthBefore = CurrentHealth;
            CurrentHealth = Mathf.Clamp(CurrentHealth + healAmount, 0, maxHealth);
            return CurrentHealth - healthBefore;
        }

        int ApplyDamage(int damageAmount)
        {
            var healthBefore = CurrentHealth;
            CurrentHealth = Mathf.Clamp(CurrentHealth - damageAmount, 0, maxHealth);
            return healthBefore - CurrentHealth;
        }
    }
}