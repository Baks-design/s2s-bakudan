using Game.Runtime.Components.Events;
using Game.Runtime.Utilities.Patterns.EventBus;
using KBCore.Refs;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Runtime.Components.UI
{
    public class SceneVisualTransition : MonoBehaviour
    {
        [SerializeField, Child] Canvas loadingCanvas;
        [SerializeField, Child] CinemachineCamera loadingCamera;
        [SerializeField, Anywhere] Image loadingBar;
        [SerializeField, Range(0.1f, 1f)] float fillSpeed = 0.5f;
        bool isLoading;
        float targetProgress;
        LoadingProgress progress;

        EventBinding<EnableLoadingCanvasEvent> enableLoadingCanvasEvent;
        EventBinding<LoadSceneTransitionEvent> loadSceneTransitionEvent;

        void Awake()
        {
            enableLoadingCanvasEvent = new EventBinding<EnableLoadingCanvasEvent>(EnableLoadingCanvas);
            EventBus<EnableLoadingCanvasEvent>.Register(enableLoadingCanvasEvent);

            loadSceneTransitionEvent = new EventBinding<LoadSceneTransitionEvent>(LoadSceneTransition);
            EventBus<LoadSceneTransitionEvent>.Register(loadSceneTransitionEvent);
        }

        void Update()
        {
            HandleFill();
            FillProgress();
        }

        void OnDisable()
        {
            EventBus<EnableLoadingCanvasEvent>.Deregister(enableLoadingCanvasEvent);
            EventBus<LoadSceneTransitionEvent>.Deregister(loadSceneTransitionEvent);
        }

        void EnableLoadingCanvas(EnableLoadingCanvasEvent enableLoadingCanvasEvent)
        {
            isLoading = enableLoadingCanvasEvent.enable;
            loadingCanvas.gameObject.SetActive(enableLoadingCanvasEvent.enable);
            loadingCamera.gameObject.SetActive(enableLoadingCanvasEvent.enable);
        }

        void LoadSceneTransition(LoadSceneTransitionEvent loadSceneTransitionEvent)
        {
            loadingBar.fillAmount = 0f;
            targetProgress = 1f;

            progress = new LoadingProgress();
            progress.Progressed += target => targetProgress = Mathf.Max(target, targetProgress);
        }

        void HandleFill()
        {
            if (!isLoading)
                return;

            var currentFillAmount = loadingBar.fillAmount;
            var progressDifference = Mathf.Abs(currentFillAmount - targetProgress);

            var dynamicFillSpeed = progressDifference * fillSpeed;

            loadingBar.fillAmount = Mathf.Lerp(currentFillAmount, targetProgress, Time.deltaTime * dynamicFillSpeed);
        }

        void FillProgress() => EventBus<LoadingProgressEvent>.Raise(new LoadingProgressEvent { progress = progress });
    }
}