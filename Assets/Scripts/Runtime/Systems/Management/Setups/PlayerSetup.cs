using UnityEngine;
using Game.Runtime.Utilities.Patterns.EventBus;
using Game.Runtime.Components.Events;

namespace Game.Runtime.Systems.Management.Setup
{
    public class PlayerSetup : MonoBehaviour
    {
        [SerializeField] GameObject playerPrefab;
        [SerializeField] Transform spawnPos;
        EventBinding<PlayerSpawnEvent> playerEventBinding;

        void OnEnable() => SubsEvents();

        void Start() => SpawnPlayer();

        void OnDisable() => UnsubsEvents();

        void SubsEvents()
        {
            playerEventBinding = new EventBinding<PlayerSpawnEvent>(LoadPlayerEvent);
            EventBus<PlayerSpawnEvent>.Register(playerEventBinding);
        }

        void UnsubsEvents() => EventBus<PlayerSpawnEvent>.Deregister(playerEventBinding);

        void LoadPlayerEvent(PlayerSpawnEvent playerEvent) => Instantiate(playerPrefab, spawnPos.position, Quaternion.identity);

        void SpawnPlayer() => EventBus<PlayerSpawnEvent>.Raise(new PlayerSpawnEvent());
    }
}