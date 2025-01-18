using Game.Runtime.Systems.Audio;
using Game.Runtime.Utilities.Patterns.ServiceLocator;
using KBCore.Refs;
using UnityEngine;

namespace Game.Runtime.Entities.Bomb
{
    public class BombSFX : MonoBehaviour
    {
        [SerializeField, Parent] Transform tr;
        [SerializeField] SoundData soundData;
        ISoundService soundService;

        void Start() => ServiceLocator.Global.Get(out soundService);

        public void PlaySFX()
        {
            var soundBuilder = soundService.CreateSoundBuilder();
            soundBuilder
                .WithRandomPitch()
                .WithPosition(tr.position)
                .Play(soundData);
        }
    }
}