using Game.Runtime.Systems.VFX;
using Game.Runtime.Utilities.Patterns.ServiceLocator;
using UnityEngine;

namespace Game.Runtime.Entities.Bomb
{
    public class BombVFX : MonoBehaviour
    {
        [SerializeField] EffectData effectData;
        IEffectService effectService;

        void Start() => ServiceLocator.Global.Get(out effectService);

        public void PlayVFX()
        {
            var effectBuilder = effectService.CreateEffectBuilder();
            effectBuilder
                .WithPosition(effectData.tr.position)
                .Play(effectData);
        }
    }
}