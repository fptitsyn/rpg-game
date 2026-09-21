using StateMachine;

namespace Actors.Enemies.States
{
    public sealed class EnemyFleeState : EnemyState
    {
        public EnemyFleeState(EnemyContext context, StateMachine.StateMachine stateMachine) : base(context, stateMachine)
        {
        }

        public override void Tick()
        {
            if (!Context.IsLowHealth)
            {
                if (Context.ShouldAggro)
                    StateMachine.Change<EnemyAggressionState>();
                else
                    StateMachine.Change<EnemyIdleState>();

                return;
            }

            Context.MoveAwayFromTarget();
        }

        public override void Exit()
        {
            Context.Stop();
        }
    }
}