using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Runtime.Systems.SceneManagement
{
    public enum SceneType { ActiveScene, MainMenu, UserInterface, HUD, Cinematic, Environment, Tooling }

    [Serializable]
    public struct SceneGroup
    {
        public string GroupName;
        public List<SceneData> Scenes;

        public readonly string FindSceneNameByType(SceneType sceneType)
        => Scenes.FirstOrDefault(scene => scene.SceneType == sceneType).Reference.Name;
    }
}