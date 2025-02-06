using System;
using UnityEngine;

namespace Game.Runtime.Systems.Serializer
{
    [Serializable]
    public struct PlayerData : ISaveable
    {
        public Vector3 position;
        public Quaternion rotation;

        public SerializableGuid Id { get; set; }
    }
}