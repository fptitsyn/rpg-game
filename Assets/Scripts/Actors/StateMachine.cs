using UnityEngine;

namespace Actors
{
    public abstract class StateMachine : MonoBehaviour
    {
        public enum State
        {
            Idle,
            Walking,
            Sprinting,
            Jumping,
            Falling,
            PhysicalAttack,
            MagicAttack,
            Blocking,
            Damaged,
            Dead
        }

        [SerializeField] protected State currentState;
        protected float StateTimer;

        public void ChangeState(State newState)
        {
            if (newState == currentState) return;

            ExitState(currentState);
            currentState = newState;
            EnterState(currentState);
        }

        protected virtual void Update()
        {
            UpdateState(currentState);
        }

        protected abstract void EnterState(State state);
        protected abstract void UpdateState(State state);
        protected abstract void ExitState(State state);
    }
}