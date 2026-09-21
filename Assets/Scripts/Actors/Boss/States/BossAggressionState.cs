using StateMachine;

namespace Actors.Boss.States
{
    public sealed class BossAggressionState : BossState
    {
        public BossAggressionState(BossContext context, StateMachine.StateMachine stateMachine) : base(context, stateMachine)
        {
        }

        public override void Enter()
        {
            Context.Stop();
        }

        public override void Tick()
        {
            if (Context.CanUseStrongAttack())
            {
                StateMachine.Change<BossStrongAttackState>();
                return;
            }

            if (Context.DistanceToTarget <= Context.Definition.meleeReach && Context.HasLineOfSight)
            {
                StateMachine.Change<BossAttackState>();
                return;
            }

            StateMachine.Change<BossChaseState>();
        }
    }
}