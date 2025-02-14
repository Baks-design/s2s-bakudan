using System;
using System.Collections.Generic;

namespace Game.Runtime.Utilities.Patterns.StateMachines
{
    public class StateMachine
    {
        StateNode currentNode;
        readonly Dictionary<Type, StateNode> nodes = new();
        readonly HashSet<Transition> anyTransitions = new();

        public IState CurrentState => currentNode.State;

        public void Update()
        {
            var transition = GetTransition();

            if (transition != null)
            {
                ChangeState(transition.To);
                ResetAllActionPredicateFlags();
            }

            currentNode.State?.Update();
        }

        void ResetAllActionPredicateFlags()
        {
            foreach (var node in nodes.Values)
                foreach (var transition in node.Transitions)
                    if (transition is Transition<ActionPredicate> actionTransition)
                        actionTransition.condition.flag = false;

            foreach (var transition in anyTransitions)
                if (transition is Transition<ActionPredicate> actionTransition)
                    actionTransition.condition.flag = false;
        }

        public void FixedUpdate() => currentNode.State?.FixedUpdate();

        public void SetState(IState state)
        {
            if (nodes.TryGetValue(state.GetType(), out var node))
            {
                currentNode = node;
                currentNode.State?.OnEnter();
            }
        }

        void ChangeState(IState state)
        {
            if (state == currentNode.State)
                return;

            var previousState = currentNode.State;
            if (nodes.TryGetValue(state.GetType(), out var nextNode))
            {
                previousState?.OnExit();
                nextNode.State.OnEnter();
                currentNode = nextNode;
            }
        }

        public void AddTransition<T>(IState from, IState to, T condition)
        => GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, condition);

        public void AddAnyTransition<T>(IState to, T condition)
        => anyTransitions.Add(new Transition<T>(GetOrAddNode(to).State, condition));

        Transition GetTransition()
        {
            foreach (var transition in anyTransitions)
                if (transition.Evaluate())
                    return transition;

            foreach (var transition in currentNode.Transitions)
                if (transition.Evaluate())
                    return transition;

            return null;
        }

        StateNode GetOrAddNode(IState state)
        {
            if (!nodes.TryGetValue(state.GetType(), out var node))
            {
                node = new StateNode(state);
                nodes[state.GetType()] = node;
            }

            return node;
        }

        class StateNode
        {
            public IState State { get; }
            public HashSet<Transition> Transitions { get; }

            public StateNode(IState state)
            {
                State = state;
                Transitions = new HashSet<Transition>();
            }

            public void AddTransition<T>(IState to, T predicate) => Transitions.Add(new Transition<T>(to, predicate));
        }
    }
}