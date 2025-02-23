using UnityEngine;
using Game.Runtime.Utilities.Patterns.EventBus;
using Game.Runtime.Components.Events;
using static Game.Runtime.Components.Events.GameStateEvent;
using Game.Runtime.Components.Inputs;

namespace Game.Runtime.Components.UI
{
    public class PauseMenu : MonoBehaviour 
    {
        [SerializeField] GameObject pauseMenuUI;
        [SerializeField] InputReader inputReader;

        void OnEnable()
        {
            inputReader.OpenMenu += OpenMenu;
            inputReader.CloseMenu += CloseMenu;
        }

        void OnDisable()
        {
            inputReader.OpenMenu -= OpenMenu;
            inputReader.CloseMenu -= CloseMenu;
        }

        public void OpenMenu()
        {
            pauseMenuUI.SetActive(true);
            inputReader.EnableUIMap();
            EventBus<GameStateEvent>.Raise(new GameStateEvent { CurrentGameState = GameState.Menu });
        }

        public void CloseMenu()
        {
            pauseMenuUI.SetActive(false);
            inputReader.EnablePlayerMap();
            EventBus<GameStateEvent>.Raise(new GameStateEvent { CurrentGameState = GameState.Gameplay });
        }

        public void Quit() => Application.Quit();
    }
}