using System;
using Game.Runtime.Utilities.Extensions;
using UnityEngine;

namespace Game.Runtime.Systems.Persistence
{
    [Serializable]
    public class PlayerData : ISaveable
    {
        public Vector3 position;
        public Quaternion rotation;

        [field: SerializeField] public SerializableGuid Id { get; set; }
    }
}