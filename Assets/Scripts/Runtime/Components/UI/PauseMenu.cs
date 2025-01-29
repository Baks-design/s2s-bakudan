using UnityEngine;
using Game.Runtime.Utilities.Patterns.EventBus;
using Game.Runtime.Systems.Events;

namespace Game.Runtime.Components.UI
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] GameObject pauseMenuUI;

        public void OpenMenu()
        {
            pauseMenuUI.SetActive(true);
            EventBus<UIEvent>.Raise(new UIEvent { });
        }

        public void CloseMenu()
        {
            pauseMenuUI.SetActive(false);
            EventBus<UIEvent>.Raise(new UIEvent { });
        }

        public void Quit() => Application.Quit();
    }
}