using System;
using System.Collections.Generic;

namespace StateMachine
{
    public class StateMachine
    {
        private readonly Dictionary<Type, IState> _states = new();
        private IState _currentState;

        public string CurrentStateName => _currentState?.GetType().Name;

        public void Add(IState state)
        {
            _states[state.GetType()] = state;
        }

        public void Change<T>() where T : IState
        {
            IState nextState = _states[typeof(T)];

            if (ReferenceEquals(_currentState, nextState))
                return;

            _currentState?.Exit();
            _currentState = nextState;
            _currentState.Enter();
        }

        public void Tick()
        {
            _currentState?.Tick();
        }
    }
}