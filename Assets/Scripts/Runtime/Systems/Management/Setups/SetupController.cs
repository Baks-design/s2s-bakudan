using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Game.Runtime.Utilities.Patterns.EventBus;
using Game.Runtime.Components.Events;
using KBCore.Refs;
using Game.Runtime.Utilities.Helpers;

namespace Game.Runtime.Systems.Management.Setup
{
    public class SetupController : MonoBehaviour
    {
        [SerializeField, Anywhere] CinemachineCamera cinemachineCamera;
        [SerializeField] Transform spawnPos;
        [SerializeField] AssetReferenceGameObject assetReference;
        EventBinding<PlayerSpawnEvent> playerEventBinding;

        void OnEnable()
        {
            SubsEvents();
            SpawnPlayer();
        }

        void OnDisable() => UnsubsEvents();

        void SubsEvents()
        {
            playerEventBinding = new EventBinding<PlayerSpawnEvent>(LoadPlayerEvent);
            EventBus<PlayerSpawnEvent>.Register(playerEventBinding);
        }

        void UnsubsEvents() => EventBus<PlayerSpawnEvent>.Deregister(playerEventBinding);

        void LoadPlayerEvent(PlayerSpawnEvent playerEvent) => SpawnPlayer();

        void SpawnPlayer()
        {
            var instantiatedObject = Helpers.InstantiateAddressableSync(assetReference, spawnPos.position, Quaternion.identity);
            if (instantiatedObject != null)
                cinemachineCamera.Target.TrackingTarget = instantiatedObject.transform;
        }
    }
}