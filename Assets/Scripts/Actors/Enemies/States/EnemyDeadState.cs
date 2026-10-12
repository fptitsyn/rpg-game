using StateMachine;

namespace Actors.Enemies.States
{
    public sealed class EnemyDeadState : EnemyState
    {
        public EnemyDeadState(EnemyContext context, StateMachine.StateMachine stateMachine) : base(context, stateMachine)
        {
        }

        public override void Enter()
        {
            Context.Stop();
        }

        public override void Tick()
        {
        }
    }
}