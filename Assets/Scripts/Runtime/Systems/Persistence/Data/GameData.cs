using System;

namespace Game.Runtime.Systems.Persistence
{
    [Serializable]
    public class GameData
    {
        public string Name;
        public string CurrentLevelName;
        public PlayerData playerData;
    }
}