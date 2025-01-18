using System;
using UnityEngine;

namespace Game.Runtime.Components.Damage
{
    public abstract class Health : MonoBehaviour, IDamageable, IHealable
    {
        [SerializeField, Range(1f, 100f)] 
        protected float maxHealth = 10f;
        [SerializeField, Range(0.1f, 1f)] 
        protected float criticalHealthRatio = 0.3f;
        protected bool isDead;

        public float CurrentHealth { get; protected set; }
        public bool Invincible { get; set; }
        public float GetRatio => CurrentHealth / maxHealth;
        public bool IsCritical => GetRatio <= criticalHealthRatio;

        public event Action<float, GameObject> OnDamaged = delegate { };
        public event Action<float> OnHealed = delegate { };
        public event Action OnDie = delegate { };

        protected virtual void Start()
        {
            CurrentHealth = maxHealth;
            isDead = false;
        }

        public virtual void Heal(float healAmount)
        {
            if (isDead)
                return;

            var healthBefore = CurrentHealth;
            CurrentHealth = Mathf.Clamp(CurrentHealth + healAmount, 0f, maxHealth);

            var trueHealAmount = CurrentHealth - healthBefore;
            if (trueHealAmount > 0f)
                OnHealed.Invoke(trueHealAmount);
        }

        public virtual void TakeDamage(float damage, GameObject damageSource)
        {
            if (Invincible || isDead)
                return;

            var healthBefore = CurrentHealth;
            CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0f, maxHealth);

            var trueDamageAmount = healthBefore - CurrentHealth;
            if (trueDamageAmount > 0f)
                OnDamaged.Invoke(trueDamageAmount, damageSource);

            if (CurrentHealth <= 0f && !isDead)
                HandleDeath();
        }

        public void Kill()
        {
            if (isDead)
                return;

            CurrentHealth = 0f;
            OnDamaged.Invoke(maxHealth, null);
            HandleDeath();
        }

        protected virtual void HandleDeath()
        {
            isDead = true;
            OnDie.Invoke();
        }
    }
}