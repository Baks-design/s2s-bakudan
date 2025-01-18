using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Game.Runtime.Systems.Audio
{
    public class MusicManager : MonoBehaviour, IMusicService
    {
        [SerializeField] List<AudioClip> initialPlaylist;
        [SerializeField] AudioMixerGroup musicMixerGroup;
        float fading;
        const float crossFadeTime = 1f;
        AudioSource current, previous;
        readonly Queue<AudioClip> playlist = new();

        void Start()
        {
            foreach (var clip in initialPlaylist)
                AddToPlaylist(clip);
        }

        public void AddToPlaylist(AudioClip clip)
        {
            playlist.Enqueue(clip);
            if (current == null && previous == null)
                PlayNextTrack();
        }

        public void Clear() => playlist.Clear();

        public void PlayNextTrack()
        {
            if (playlist.TryDequeue(out AudioClip nextTrack))
                Play(nextTrack);
        }

        public void Play(AudioClip clip)
        {
            if (current && current.clip == clip) return;

            if (previous)
            {
                Destroy(previous);
                previous = null;
            }

            previous = current;

            //current = gameObject.GetOrAdd<AudioSource>();
            current.clip = clip;
            current.outputAudioMixerGroup = musicMixerGroup; // Set mixer group
            current.loop = false; // For playlist functionality, we want tracks to play once
            current.volume = 0;
            current.bypassListenerEffects = true;
            current.Play();

            fading = 0.001f;
        }

        void Update()
        {
            HandleCrossFade();

            if (current && !current.isPlaying && playlist.Count > 0)
                PlayNextTrack();
        }

        void HandleCrossFade()
        {
            if (fading <= 0f) return;

            fading += Time.deltaTime;

            var fraction = Mathf.Clamp01(fading / crossFadeTime);

            // Logarithmic fade
            var logFraction = 0f;/// = fraction.ToLogarithmicFraction();

            if (previous) previous.volume = 1.0f - logFraction;
            if (current) current.volume = logFraction;

            if (fraction >= 1)
            {
                fading = 0.0f;
                if (previous)
                {
                    Destroy(previous);
                    previous = null;
                }
            }
        }
    }
}