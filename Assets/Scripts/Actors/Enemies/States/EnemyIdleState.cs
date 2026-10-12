using StateMachine;

namespace Actors.Enemies.States
{
    public sealed class EnemyIdleState : EnemyState
    {
        public EnemyIdleState(EnemyContext context, StateMachine.StateMachine stateMachine) : base(context, stateMachine)
        {
        }

        public override void Enter()
        {
            Context.Stop();
        }

        public override void Tick()
        {
            if (Context.IsLowHealth)
            {
                StateMachine.Change<EnemyFleeState>();
                return;
            }

            if (Context.ShouldAggro)
                StateMachine.Change<EnemyAggressionState>();
        }
    }
}