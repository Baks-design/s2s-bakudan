using System;
using System.Collections.Generic;
using Game.Runtime.Entities.Player.Components;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.Audio;

namespace Game.Runtime.Entities.Player.Controllers
{
    public class SoundController : MonoBehaviour
    {
        [SerializeField, Parent] MovementController movement;
        [SerializeField, Parent] DetectionController detection;
        [SerializeField, Range(0.1f, 1f)] float footstepInterval = 0.5f;
        [SerializeField] AudioSource footstepAudioSource, landedFootstepAudioSource, landedSharedAudioSource;
        [SerializeField] AudioResource[] audioResources;
        [SerializeField] AudioResource landedSharedAudioResource;
        float footstepTimer = 0f;
        Dictionary<SurfaceTypeTag, AudioResource> tagToResourceMap;
        readonly SurfaceTypeTag[] cachedSurfaceTags = (SurfaceTypeTag[])Enum.GetValues(typeof(SurfaceTypeTag));
        readonly SurfaceTypeTag[] tagHandles =
        {
            SurfaceTypeTag.Concrete,
            SurfaceTypeTag.Gravel,
            SurfaceTypeTag.Grass,
            SurfaceTypeTag.Water
        };

        void OnEnable() => SubsEvents();

        void Start()
        {
            InitializeAudioResources();
            InitializeTagToResourceMap();
        }

        void Update() => DetectSurfaceForFootstepSound();

        void LateUpdate() => HandleFootsteps();

        void OnDisable() => UnsubsEvents();

        void SubsEvents() => movement.Landed += HandleLand;

        void UnsubsEvents() => movement.Landed -= HandleLand;

        void InitializeAudioResources() => landedSharedAudioSource.resource = landedSharedAudioResource;

        void InitializeTagToResourceMap()
        {
            tagToResourceMap = new Dictionary<SurfaceTypeTag, AudioResource>();
            for (var i = 0; i < tagHandles.Length && i < audioResources.Length; i++)
                tagToResourceMap[tagHandles[i]] = audioResources[i];
        }

        void HandleLand() => landedSharedAudioSource.Play();

        void HandleFootsteps()
        {
            if (!detection.IsGrounded || !movement.IsMoving) return;

            footstepTimer += Time.deltaTime;
            if (footstepTimer >= footstepInterval)
            {
                footstepTimer = 0f;
                footstepAudioSource.Play();
            }
        }

        void DetectSurfaceForFootstepSound()
        {
            if (!detection.IsGrounded || !detection.GroundCollider.TryGetComponent(out AudioTagObject tagObject)) return;

            foreach (var surfaceTypeTag in cachedSurfaceTags)
            {
                if (surfaceTypeTag is SurfaceTypeTag.None)
                    continue;

                if ((tagObject.SurfaceTypeTag & surfaceTypeTag) != surfaceTypeTag ||
                    !tagToResourceMap.TryGetValue(surfaceTypeTag, out var newResource))
                    continue;

                if (footstepAudioSource.resource != newResource)
                    SetNewFootstepResource(newResource);

                return;
            }
        }

        void SetNewFootstepResource(AudioResource newResource)
        {
            footstepAudioSource.Stop();
            footstepAudioSource.resource = newResource;
            landedFootstepAudioSource.resource = newResource;
            footstepAudioSource.Play();
        }
    }
}