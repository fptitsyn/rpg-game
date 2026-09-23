using StateMachine;

namespace Actors.Enemies.States
{
    public sealed class EnemyAggressionState : EnemyState
    {
        public EnemyAggressionState(EnemyContext context, StateMachine.StateMachine stateMachine) : base(context, stateMachine)
        {
        }

        public override void Tick()
        {
            if (Context.IsLowHealth)
            {
                StateMachine.Change<EnemyFleeState>();
                return;
            }

            if (Context.Mode == EnemyMode.Peaceful)
            {
                StateMachine.Change<EnemyIdleState>();
                return;
            }

            if (Context.DistanceToTarget > Context.Definition.loseTargetDistance)
            {
                Context.ForgetTarget();
                StateMachine.Change<EnemyIdleState>();
                return;
            }

            if (Context.CanAttack())
            {
                if (Context.RequiresPreparation)
                    StateMachine.Change<EnemyPrepareAttackState>();
                else
                    StateMachine.Change<EnemyAttackState>();

                return;
            }

            if (Context.Ranged && Context.DistanceToTarget < Context.Definition.rangedMinDistance)
            {
                Context.MoveAwayFromTarget();
                return;
            }

            Context.MoveToTarget();
        }
    }
}