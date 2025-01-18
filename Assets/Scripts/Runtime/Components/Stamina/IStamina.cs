namespace Game.Runtime.Components.Stamina
{
    public interface IStamina
    {
        float CurrentStamina { get; }
        bool IsExhausted { get; }

        void UseStamina(float amount);
        void RegenerateStamina();
    }
}