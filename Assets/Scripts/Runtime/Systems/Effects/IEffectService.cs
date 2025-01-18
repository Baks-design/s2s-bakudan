namespace Game.Runtime.Systems.VFX
{
    public interface IEffectService
    {
        EffectBuilder CreateEffectBuilder();
        void ReturnToPool(EffectEmitter emitter);
    }
}