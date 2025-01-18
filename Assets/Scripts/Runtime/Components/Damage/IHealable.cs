using System;

namespace Game.Runtime.Components.Damage
{
    public interface IHealable
    {
        event Action<float> OnHealed;

        void Heal(float healAmount);
    }
}