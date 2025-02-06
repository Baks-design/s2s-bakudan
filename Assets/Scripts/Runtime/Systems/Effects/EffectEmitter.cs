using System.Collections;
using Game.Runtime.Utilities.Patterns.ServiceLocator;
using UnityEngine;
using UnityEngine.VFX;

namespace Game.Runtime.Systems.VFX
{
    public class EffectEmitter : MonoBehaviour
    {
        [SerializeField] VisualEffect visualEffect;
        Coroutine playingCoroutine;
        IEffectService effectService;

        void Start() => ServiceLocator.Global.Get(out effectService);

        public void Initialize(EffectData data)
        {
            visualEffect.visualEffectAsset = data.vfxAsset;
            visualEffect.SetVector3("Position", data.tr.position);
            visualEffect.SetFloat("Duration", data.duration);
        }

        public void Play()
        {
            if (playingCoroutine != null)
                StopCoroutine(playingCoroutine);

            visualEffect.Play();
            playingCoroutine = StartCoroutine(WaitForSoundToEnd());
        }

        IEnumerator WaitForSoundToEnd()
        {
            yield return new WaitWhile(() => visualEffect.HasAnySystemAwake());
            Stop();
        }

        public void Stop()
        {
            if (playingCoroutine != null)
            {
                StopCoroutine(playingCoroutine);
                playingCoroutine = null;
            }

            visualEffect.Stop();
            effectService.ReturnToPool(this);
        }
    }
}