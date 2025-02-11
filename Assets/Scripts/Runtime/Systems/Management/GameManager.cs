using UnityEngine;
using Game.Runtime.Utilities.Patterns.StateMachines;
using Game.Runtime.Utilities.Patterns.EventBus;
using Game.Runtime.Systems.Inputs;
using Game.Runtime.Components.Events;
using Game.Runtime.Systems.Management.States;
using Game.Runtime.Utilities.Helpers.Timers;

namespace Game.Runtime.Systems.Management
{
    public class GameManager : StatefulEntity, IGameService 
    {
        [SerializeField] InputReader inputReader;
        [SerializeField] bool isShowDebug = true;
        [SerializeField] Rect stateDebugText = new(10f, 10f, 200f, 20f);
        GameStateEvent.GameState requestedState;
        EventBinding<GameStateEvent> gameStateEventBinding;
        readonly CountdownTimer timer = new(200f);

        bool GameplayState => requestedState is GameStateEvent.GameState.Gameplay;
        bool MenuState => requestedState is GameStateEvent.GameState.Menu;

        #region LifeCycle
        protected override void Awake()
        {
            base.Awake();
            SetupStates();
        }

        void OnEnable() => SubEvents();

        void Start()
        {
            InitializePlayer();
            InitializeGameTimer();
            InitInput();
        }

        protected override void FixedUpdate() => base.FixedUpdate();

        protected override void Update()
        {
            base.Update();
            UpdateGameTime();
        }

        void OnDisable() => UnsubEvents();

        void OnDestroy() => DisposeItems();

        void OnGUI()
        {
            if (!isShowDebug) return;
            GUI.Label(stateDebugText, $"Current Game State: {StateMachine.CurrentState}");
        }
        #endregion

        #region Inits
        void SetupStates() //FIXME: Adjust
        {
            var gameplayState = new GameplayState(this);
            var pauseState = new PauseState(this);

            At(pauseState, gameplayState, GameplayState);
            At(gameplayState, pauseState, MenuState);

            StateMachine.SetState(gameplayState); //TODO: Change When Use Menus To Menu State
        }

        void InitializeGameTimer() => timer.OnTimerStart += () => Debug.Log("Game Timer started");

        void DisposeItems() => timer.Dispose();
        #endregion

        #region Events
        void SubEvents()
        {
            gameStateEventBinding = new EventBinding<GameStateEvent>(OnChangeStateRequested);
            EventBus<GameStateEvent>.Register(gameStateEventBinding);
        }

        void UnsubEvents() => EventBus<GameStateEvent>.Deregister(gameStateEventBinding);

        void OnChangeStateRequested(GameStateEvent gameStateEvent) => requestedState = gameStateEvent.CurrentGameState;
        #endregion

        #region Updates
        void UpdateGameTime() => EventBus<GameStateEvent>.Raise(new GameStateEvent { CurrentGameTime = timer.Progress });
        #endregion

        #region Providers
        void InitializePlayer() => EventBus<PlayerEvent>.Raise(new PlayerEvent());

        void InitInput() => inputReader.EnablePlayerMap();
        #endregion
    }
}