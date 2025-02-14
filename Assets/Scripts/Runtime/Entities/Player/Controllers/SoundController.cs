using System.Collections.Generic;
using Game.Runtime.Entities.Player.Components;
using Game.Runtime.Utilities.Helpers;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.Audio;

namespace Game.Runtime.Entities.Player.Controllers
{
    public class SoundController : MonoBehaviour
    {
        [SerializeField, Self] Transform tr;
        [SerializeField, Parent] PlayerMover _playerMover;
        [SerializeField, Parent] MovementController _movementController;
        [SerializeField] float footstepDistance = 1.5f;
        [SerializeField] AudioSource _footstepAudioSource;
        [SerializeField] AudioSource _landedFootstepAudioSource;
        [SerializeField] AudioSource _landedSharedAudioSource;
        [SerializeField] AudioResource[] _audioResources;
        [SerializeField] AudioResource _landedSharedAudioResource;
        float _distanceMoved;
        Vector3 _lastPosition;
        Dictionary<SurfaceTypeTag, AudioResource> _tagToResourceMap;
        readonly SurfaceTypeTag[] _tagHandles =
        {
            SurfaceTypeTag.Concrete,
            SurfaceTypeTag.Gravel,
            SurfaceTypeTag.Grass,
            SurfaceTypeTag.Water
        };

        void OnEnable() => SubscribeEvents();

        void Start()
        {
            InitializeAudioResources();
            InitializeTagToResourceMap();
            _lastPosition = tr.position;
        }

        void Update() => HandleFootsteps();

        void OnDisable() => UnsubscribeEvents();

        void SubscribeEvents() => _movementController.OnLand += HandleLand;

        void UnsubscribeEvents() => _movementController.OnLand -= HandleLand;

        void HandleLand(Vector3 _) => _landedSharedAudioSource.Play();

        void InitializeAudioResources() => _landedSharedAudioSource.resource = _landedSharedAudioResource;

        void InitializeTagToResourceMap()
        {
            _tagToResourceMap = new Dictionary<SurfaceTypeTag, AudioResource>();
            for (var i = 0; i < _tagHandles.Length && i < _audioResources.Length; i++)
                _tagToResourceMap[_tagHandles[i]] = _audioResources[i];
        }

        void HandleFootsteps()
        {
            var isGrounded = _playerMover.IsGrounded;
            var movementVelocityMagnitude = _movementController.GetMovementVelocity.magnitude;

            if (isGrounded && movementVelocityMagnitude > 0.1f)
            {
                PlayFootsteps();
                DetectSurfaceForFootstepSound();
            }
        }

        void PlayFootsteps()
        {
            _distanceMoved += Vector3.Distance(tr.position, _lastPosition);
            _lastPosition = tr.position;

            if (_distanceMoved >= footstepDistance)
            {
                _footstepAudioSource.Play();
                _distanceMoved = 0f;
            }
        }

        void DetectSurfaceForFootstepSound()
        {
            if (!_playerMover.GetColliderHit.TryGetComponent(out AudioTagObject tagObject))
                return;

            foreach (var surfaceTypeTag in _tagHandles)
            {
                if ((tagObject.SurfaceTypeTag & surfaceTypeTag) == surfaceTypeTag &&
                    _tagToResourceMap.TryGetValue(surfaceTypeTag, out var newResource))
                {
                    if (_footstepAudioSource.resource != newResource)
                        SetNewFootstepResource(newResource);
                    break;
                }
            }
        }

        void SetNewFootstepResource(AudioResource newResource)
        {
            _footstepAudioSource.Stop();
            _footstepAudioSource.resource = newResource;
            _landedFootstepAudioSource.resource = newResource;
            _footstepAudioSource.Play();
        }
    }
}