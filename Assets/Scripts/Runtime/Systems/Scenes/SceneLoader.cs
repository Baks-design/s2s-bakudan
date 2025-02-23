using System;
using System.Threading.Tasks;
using Game.Runtime.Components.Events;
using Game.Runtime.Utilities.Patterns.EventBus;
using UnityEngine;

namespace Game.Runtime.Systems.Scenes
{
    public enum SceneID
    {
        NewGame, LoadGame, Menu, Level01
    }

    public class SceneLoader : MonoBehaviour, ISceneLoaderService
    {
        [SerializeField] SceneGroup[] groups;
        IProgress<float> visual;
        readonly SceneGroupManager manager = new();

        EventBinding<LoadingProgressEvent> loadingProgressEvent;

        async void Awake() => await LoadSceneGroup(0);

        void OnEnable()
        {
            loadingProgressEvent = new EventBinding<LoadingProgressEvent>(LoadingProgress);
            EventBus<LoadingProgressEvent>.Register(loadingProgressEvent);
        }

        void OnDisable() => EventBus<LoadingProgressEvent>.Deregister(loadingProgressEvent);

        void LoadingProgress(LoadingProgressEvent progressEvent) => visual = progressEvent.progress;

        public async Task LoadSceneGroup(int index)
        {
            EventBus<LoadSceneTransitionEvent>.Raise(new LoadSceneTransitionEvent());
            EventBus<EnableLoadingCanvasEvent>.Raise(new EnableLoadingCanvasEvent { enable = true });
            await manager.LoadScenes(groups[index], visual);
            EventBus<EnableLoadingCanvasEvent>.Raise(new EnableLoadingCanvasEvent { enable = false });
        }
    }
}