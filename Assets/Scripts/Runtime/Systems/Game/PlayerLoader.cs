using Unity.Cinemachine;
using UnityEngine;
using Game.Runtime.Utilities.Patterns.EventBus;
using Game.Runtime.Systems.Events;
using Game.Runtime.Components.UI.Minimap;

namespace Game.Runtime.Systems.GameManagement
{
    public class PlayerLoader : MonoBehaviour
    {
        [SerializeField] GameObject playerPrefab;
        [SerializeField] CinemachineCamera vcam;
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
            SetupCamera();
            SetupMinimap();
        }

        void InstantiatePlayer() => player = Instantiate(playerPrefab, spawnPos.position, Quaternion.identity);

        void SetupCamera()
        {
            vcam.Target.TrackingTarget = player.transform;
            vcam.OnTargetObjectWarped(player.transform, player.transform.position - vcam.transform.position - Vector3.forward);
        }

        void SetupMinimap()
        {
            var minimap = FindFirstObjectByType<MiniMapView>();
            var img = minimap.FollowCentered(player.transform);
            img.color = new Color(255f, 255f, 0f);
        }
    }
}