using System;

namespace Game.Runtime.Components.Damage
{
    public interface IHealable
    {
        event Action<int> OnHealed;

        void Heal(int healAmount);
    }
}