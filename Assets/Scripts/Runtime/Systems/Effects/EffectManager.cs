using UnityEngine;
using UnityEngine.Pool;

namespace Game.Runtime.Systems.VFX
{
    public class EffectManager : MonoBehaviour, IEffectService
    {
        [SerializeField] EffectEmitter effectEmitterPrefab;
        [SerializeField] bool collectionCheck = true;
        [SerializeField] int defaultCapacity = 10;
        [SerializeField] int maxPoolSize = 100;
        IObjectPool<EffectEmitter> effectEmitterPool;

        void Start() => InitializePools();

        public EffectBuilder CreateEffectBuilder() => new(this);

        void InitializePools() => effectEmitterPool = new ObjectPool<EffectEmitter>(
            CreateVisualEffectEmitter, OnTakeFromPool, OnReturnedToPool, DestroyEmitter,
            collectionCheck, defaultCapacity, maxPoolSize);

        EffectEmitter CreateVisualEffectEmitter() => Instantiate(effectEmitterPrefab);

        void OnTakeFromPool(EffectEmitter emitter) => emitter.gameObject.SetActive(true);

        void OnReturnedToPool(EffectEmitter emitter) => emitter.gameObject.SetActive(false);

        void DestroyEmitter(EffectEmitter emitter) => Destroy(emitter.gameObject);

        public EffectEmitter GetEmitter() => effectEmitterPool.Get();

        public void ReturnToPool(EffectEmitter emitter)
        {
            if (emitter is EffectEmitter visualEffectEmitter)
                effectEmitterPool.Release(visualEffectEmitter);
        }
    }
}