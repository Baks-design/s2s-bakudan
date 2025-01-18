using UnityEngine;

namespace Game.Runtime.Components.Stamina
{
    public class PlayerStamina : Stamina
    {
        public override void Start() => base.Start();

        public override void UseStamina(float amount) => base.UseStamina(amount);

        public override void RegenerateStamina() => base.RegenerateStamina();

        public void HandleStamina(bool isRunning, float consumingAmount)
        {
            if (isRunning)
                UseStamina(consumingAmount * Time.deltaTime);
            else
                RegenerateStamina();
        }
    }
}