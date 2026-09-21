using StateMachine;

namespace Actors.Boss.States
{
    public abstract class BossState : IState
    {
        protected readonly BossContext Context;
        protected readonly StateMachine.StateMachine StateMachine;

        protected BossState(BossContext context, StateMachine.StateMachine stateMachine)
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