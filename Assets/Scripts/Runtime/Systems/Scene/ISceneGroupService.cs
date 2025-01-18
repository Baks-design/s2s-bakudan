using System;
using System.Threading.Tasks;

namespace Game.Runtime.Systems.SceneManagement
{
    public interface ISceneGroupService
    {
        public Task LoadScenes(SceneGroup group, IProgress<float> progress, bool reloadDupScenes = false);
        public Task UnloadScenes();
    }
}