using StateMachine;

namespace Actors.Enemies.States
{
    public abstract class EnemyState : IState
    {
        protected readonly EnemyContext Context;
        protected readonly StateMachine.StateMachine StateMachine;

        protected EnemyState(EnemyContext context, StateMachine.StateMachine stateMachine)
        {
            Context = context;
            StateMachine = stateMachine;
        }

        public virtual void Enter()
        {
        }

        public abstract void Tick();

        public virtual void Exit()
        {
        }
    }
}