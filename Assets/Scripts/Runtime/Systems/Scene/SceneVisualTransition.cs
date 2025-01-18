using KBCore.Refs;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Runtime.Systems.SceneManagement
{
    public class SceneVisualTransition : MonoBehaviour
    {
        [SerializeField, Child] Canvas loadingCanvas;
        [SerializeField, Child] CinemachineCamera loadingCamera;
        [SerializeField, Anywhere] Image loadingBar;
        [SerializeField] float fillSpeed = 0.5f;
        float targetProgress;
        bool isLoading;
        LoadingProgress progress;

        public LoadingProgress Progress => progress;

        void Update()
        {
            if (!isLoading)
                return;

            var currentFillAmount = loadingBar.fillAmount;
            var progressDifference = Mathf.Abs(currentFillAmount - targetProgress);

            var dynamicFillSpeed = progressDifference * fillSpeed;

            loadingBar.fillAmount = Mathf.Lerp(currentFillAmount, targetProgress, Time.deltaTime * dynamicFillSpeed);
        }

        public void LoadSceneTransition()
        {
            loadingBar.fillAmount = 0f;
            targetProgress = 1f;

            progress = new LoadingProgress();
            progress.Progressed += target => targetProgress = Mathf.Max(target, targetProgress);
        }

        public void EnableLoadingCanvas(bool enable = true)
        {
            isLoading = enable;
            loadingCanvas.gameObject.SetActive(enable);
            loadingCamera.gameObject.SetActive(enable);
        }
    }
}