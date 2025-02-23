using Game.Runtime.Utilities.Patterns.ServiceLocator;
using UnityEngine;
using UnityEngine.VFX;

namespace Game.Runtime.Systems.VFX
{
    public class EffectEmitter : MonoBehaviour
    {
        [SerializeField] VisualEffect visualEffect;
        IEffectService effectService;

        void Awake() => ServiceLocator.Global.Get(out effectService);

        public void Initialize(EffectData data)
        {
            visualEffect.visualEffectAsset = data.vfxAsset;
            visualEffect.SetVector3("Position", data.tr.position);
            visualEffect.SetFloat("Duration", data.duration);
        }

        public async void Play()
        {
            visualEffect.Play();
            await WaitForSoundToEnd();
            Stop();
        }

        async Awaitable WaitForSoundToEnd()
        {
            while (visualEffect.HasAnySystemAwake())
                await Awaitable.NextFrameAsync();
        }

        public void Stop()
        {
            visualEffect.Stop();
            effectService.ReturnToPool(this);
        }
    }
}