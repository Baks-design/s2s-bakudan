using UnityEngine;
using Game.Runtime.Systems.GameManagement.States;
using Game.Runtime.Utilities.Patterns.StateMachines;
using Game.Runtime.Utilities.Patterns.EventBus;
using Game.Runtime.Systems.Events;
using Game.Runtime.Systems.SceneManagement;
using Game.Runtime.Utilities.Patterns.ServiceLocator;

namespace Game.Runtime.Systems.GameManagement
{
    public class GameManager : StatefulEntity, IGameService
    {
        [SerializeField] InputReader inputReader;
        bool changeToPause;
        bool changeToGameplay;
        ISceneLoaderService loaderService;
        EventBinding<UIEvent> uiPauseEventBinding;
        EventBinding<UIEvent> uiGameplayEventBinding;

        public InputReader InputReader => inputReader;

        protected override void Awake()
        {
            base.Awake();
            InputActions();
            SubsEvents(); 
        }

        void OnEnable()
        {
            InitServices();
            CreateStateMachine();
        }

        void Start() => InitializePlayer();

        protected override void FixedUpdate() => base.FixedUpdate();

        protected override void Update()
        {
            base.Update();
            Debug.Log($"Game Current State: {StateMachine.CurrentState}");
        }

        void OnDisable() => UnsubsEvents();

        async void LoadScenes() => await loaderService.LoadSceneGroup(0);

        void InputActions() => inputReader.EnableInputActions();

        void InitServices() => ServiceLocator.Global.Get(out loaderService);

        void CreateStateMachine()
        {
            var gameplayState = new GameplayState(this);
            var pauseState = new PauseState(this);

            At(gameplayState, pauseState, changeToPause);
            At(pauseState, gameplayState, changeToGameplay);

            StateMachine.SetState(gameplayState);
        }

        void InitializePlayer() => EventBus<PlayerEvent>.Raise(new PlayerEvent());

        void SubsEvents()
        {
            uiPauseEventBinding = new EventBinding<UIEvent>(OnPauseStateRequested);
            EventBus<UIEvent>.Register(uiPauseEventBinding);

            uiGameplayEventBinding = new EventBinding<UIEvent>(OnGameplayStateRequested);
            EventBus<UIEvent>.Register(uiGameplayEventBinding);
        }

        void UnsubsEvents()
        {
            EventBus<UIEvent>.Deregister(uiPauseEventBinding);
            EventBus<UIEvent>.Deregister(uiGameplayEventBinding);
        }

        void OnPauseStateRequested(UIEvent uiEvent)
        {
            changeToPause = true;
            changeToGameplay = false;
        }

        void OnGameplayStateRequested(UIEvent uiEvent)
        {
            changeToGameplay = true;
            changeToPause = false;
        }

        public static void SetCursorLock(bool isLocked)
        {
            Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !isLocked;
        }
    }
}