using UnityEngine;
using Game.Runtime.Utilities.Patterns.StateMachines;
using Game.Runtime.Utilities.Patterns.EventBus;
using Game.Runtime.Components.Events;
using Game.Runtime.Systems.Management.States;

namespace Game.Runtime.Systems.Management
{
    public class GameManager : StatefulEntity, IGameService
    {
        GameStateEvent.GameState requestedState = GameStateEvent.GameState.Gameplay;
        EventBinding<GameStateEvent> gameStateEventBinding;

        bool RequestedGameplayState => requestedState is GameStateEvent.GameState.Gameplay;
        bool RequestedMenuState => requestedState is GameStateEvent.GameState.Menu;

        #region LifeCycle
        protected override void Awake()
        {
            base.Awake();
            SetupStates();
        }

        void OnEnable() => SubEvents();

        void OnDisable() => UnsubEvents();
        #endregion

        #region Setups
        void SetupStates()
        {
            var gameplayState = new GameplayState(this);
            var pauseState = new PauseState(this);

            At(pauseState, gameplayState, RequestedGameplayState);
            At(gameplayState, pauseState, RequestedMenuState);

            StateMachine.SetState(gameplayState);
        }
        #endregion

        #region Events
        void SubEvents()
        {
            gameStateEventBinding = new EventBinding<GameStateEvent>(OnChangeStateRequested);
            EventBus<GameStateEvent>.Register(gameStateEventBinding);
        }

        void UnsubEvents() => EventBus<GameStateEvent>.Deregister(gameStateEventBinding);

        void OnChangeStateRequested(GameStateEvent gameStateEvent)
        {
            Debug.Log($"State Change Requested: {gameStateEvent.CurrentGameState}");
            requestedState = gameStateEvent.CurrentGameState;
        }
        #endregion
    }
}