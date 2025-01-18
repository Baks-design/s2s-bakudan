using UnityEngine;

namespace Game.Runtime.Components.Stamina
{
    public abstract class Stamina : MonoBehaviour, IStamina
    {
        [SerializeField, Range(1f, 100f)] protected float maxStamina = 100f;
        [SerializeField, Range(0.1f, 1f)]  protected float staminaRegenRate = 0.3f;
        bool isExhausted;
        float currentStamina;

        public float CurrentStamina => currentStamina;
        public float MaxStamina => maxStamina;
        public bool IsExhausted => isExhausted;

        public virtual void Start() => currentStamina = maxStamina;

        public virtual void UseStamina(float amount)
        {
            if (currentStamina - amount < 0f)
            {
                currentStamina = 0f;
                isExhausted = true;
            }
            else
            {
                currentStamina -= amount;
                isExhausted = false;
            }
        }

        public virtual void RegenerateStamina()
        {
            if (currentStamina >= maxStamina)
                return;

            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
            isExhausted = false;
        }
    }
}