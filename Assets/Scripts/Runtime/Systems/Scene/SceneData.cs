using System;
using Eflatun.SceneReference;

namespace Game.Runtime.Systems.SceneManagement
{
    [Serializable]
    public struct SceneData
    {
        public SceneReference Reference;
        public SceneType SceneType;

        public readonly string Name => Reference.Name;
    }
}