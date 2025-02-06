using UnityEngine;
using Game.Runtime.Utilities.Patterns.EventBus;
using Game.Runtime.Components.Events;
using Game.Runtime.Components.UI;

namespace Game.Runtime.Systems.Management.Setup
{
    public class PlayerSetup : MonoBehaviour
    {
        [SerializeField] GameObject playerPrefab;
        [SerializeField] Transform spawnPos;
        GameObject player;
        EventBinding<PlayerEvent> playerEventBinding;

        void Awake() => SubsEvents();

        void OnDisable() => UnsubsEvents();

        void SubsEvents()
        {
            playerEventBinding = new EventBinding<PlayerEvent>(LoadPlayerEvent);
            EventBus<PlayerEvent>.Register(playerEventBinding);
        }

        void UnsubsEvents() => EventBus<PlayerEvent>.Deregister(playerEventBinding);

        void LoadPlayerEvent(PlayerEvent playerEvent)
        {
            InstantiatePlayer();
            SetupMinimap();
        }

        void InstantiatePlayer() => player = Instantiate(playerPrefab, spawnPos.position, Quaternion.identity);

        void SetupMinimap()
        {
            var minimap = FindFirstObjectByType<MiniMapView>();
            var img = minimap.FollowCentered(player.transform);
            img.color = new Color(255f, 255f, 0f);
        }
    }
}