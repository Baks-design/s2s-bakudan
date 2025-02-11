using UnityEngine;
using Game.Runtime.Utilities.Patterns.EventBus;
using Game.Runtime.Components.Events;

namespace Game.Runtime.Systems.Management.Setup
{
    public class PlayerSetup : MonoBehaviour //BUG: Fix Multiples Spawns
    {
        [SerializeField] GameObject playerPrefab;
        [SerializeField] Transform spawnPos;
        EventBinding<PlayerEvent> playerEventBinding;

        void OnEnable() => SubsEvents();

        void OnDisable() => UnsubsEvents();

        void SubsEvents()
        {
            playerEventBinding = new EventBinding<PlayerEvent>(LoadPlayerEvent);
            EventBus<PlayerEvent>.Register(playerEventBinding);
        }

        void UnsubsEvents() => EventBus<PlayerEvent>.Deregister(playerEventBinding);

        void LoadPlayerEvent(PlayerEvent playerEvent) => Instantiate(playerPrefab, spawnPos.position, Quaternion.identity);
    }
}