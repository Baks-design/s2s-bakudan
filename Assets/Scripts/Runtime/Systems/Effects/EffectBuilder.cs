using UnityEngine;

namespace Game.Runtime.Systems.VFX
{
    public class EffectBuilder
    {
        readonly EffectManager effectManager;
        Vector3 position = Vector3.zero;

        public EffectBuilder(EffectManager effectManager) => this.effectManager = effectManager;

        public EffectBuilder WithPosition(Vector3 position)
        {
            this.position = position;
            return this;
        }

        public void Play(EffectData effectData)
        {
            if (effectData == null)
            {
                Debug.LogError("EffectData is null");
                return;
            }

            var emitter = effectManager.GetEmitter();
            emitter.transform.position = position;
            emitter.Initialize(effectData);
            emitter.Play();
        }
    }
}