using System;
using Game.Runtime.Systems.Scenes;

namespace Game.Runtime.Systems.Persistence
{
    [Serializable]
    public class GameData
    {
        public string Name;
        public SceneID CurrentLevelName;
        public PlayerData playerData;
    }
}