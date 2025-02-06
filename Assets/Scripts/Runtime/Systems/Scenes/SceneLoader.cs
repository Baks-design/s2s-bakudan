using System.Threading.Tasks;
using Game.Runtime.Components.UI;
using KBCore.Refs;
using UnityEngine;

namespace Game.Runtime.Systems.Scenes
{
    public class SceneLoader : MonoBehaviour, ISceneLoaderService
    {
        [SerializeField, Self] SceneVisualTransition visual;
        [SerializeField] SceneGroup[] groups;
        readonly SceneGroupManager manager = new();

        async void Awake() => await LoadSceneGroup(0);

        public async Task LoadSceneGroup(int index)
        {
            visual.LoadSceneTransition();
            visual.EnableLoadingCanvas();
            await manager.LoadScenes(groups[index], visual.Progress);
            visual.EnableLoadingCanvas(false);
        }
    }
}