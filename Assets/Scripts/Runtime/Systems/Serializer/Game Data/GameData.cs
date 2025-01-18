using System;

namespace Game.Runtime.Systems.Serializer
{
    [Serializable]
    public struct GameData
    {
        public string Name;
        public string CurrentLevelName;
        public PlayerData playerData;
    }
}