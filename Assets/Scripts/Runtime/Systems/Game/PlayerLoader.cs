using Unity.Cinemachine;
using UnityEngine;
using Game.Runtime.Utilities.Patterns.EventBus;
using Game.Runtime.Systems.Events;

namespace Game.Runtime.Systems.GameManagement
{
    public class PlayerLoader : MonoBehaviour
    {
        [SerializeField] GameObject playerPrefab;
        [SerializeField] CinemachineCamera vcam;
        [SerializeField] Transform spawnPos;
        EventBinding<PlayerEvent> playerEventBinding;

        void Awake()
        {
            playerEventBinding = new EventBinding<PlayerEvent>(LoadPlayerEvent);
            EventBus<PlayerEvent>.Register(playerEventBinding);
        }

        void OnDisable() => EventBus<PlayerEvent>.Deregister(playerEventBinding);

        void LoadPlayerEvent(PlayerEvent playerEvent)
        {
            var player = Instantiate(playerPrefab, spawnPos.position, Quaternion.identity);
            vcam.Target.TrackingTarget = player.transform;
            vcam.OnTargetObjectWarped(player.transform, player.transform.position - vcam.transform.position - Vector3.forward);
        }
    }
}