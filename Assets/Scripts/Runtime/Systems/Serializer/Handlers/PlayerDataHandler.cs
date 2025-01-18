using UnityEngine;
using KBCore.Refs;

namespace Game.Runtime.Systems.Serializer
{
    public class PlayerDataHandler : MonoBehaviour, IBind<PlayerData>
    {
        [SerializeField, Self] Transform t;
        [SerializeField] PlayerData playerData;

        public SerializableGuid ID { get; set; } = SerializableGuid.NewGuid;

        void Update() => HandleData();

        void HandleData()
        {
            playerData.position = t.position;
            playerData.rotation = t.rotation;
        }

        public void Bind(PlayerData playerData)
        {
            this.playerData = playerData;
            this.playerData.Id = ID;
            t.SetPositionAndRotation(playerData.position, playerData.rotation);
        }
    }
}