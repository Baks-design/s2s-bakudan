namespace Game.Runtime.Systems.Audio
{
    public interface ISoundService
    {
        SoundBuilder CreateSoundBuilder();
        
        void ReturnToPool(SoundEmitter soundEmitter);
    }
}