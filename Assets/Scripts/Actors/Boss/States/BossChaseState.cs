using Actors.Enemies;
using StateMachine;

namespace Actors.Boss.States
{
    public sealed class BossChaseState : BossState
    {
        public BossChaseState(BossContext context, StateMachine.StateMachine stateMachine) : base(context, stateMachine)
        {
        }

        public override void Tick()
        {
            if (Context.Mode == EnemyMode.Normal &&
                Context.DistanceToTarget > Context.Definition.loseTargetDistance)
            {
                Context.ForgetTarget();
                StateMachine.Change<BossIdleState>();
                return;
            }

            if (Context.CanUseStrongAttack() ||
                Context.DistanceToTarget <= Context.Definition.meleeReach && Context.HasLineOfSight)
            {
                StateMachine.Change<BossAggressionState>();
                return;
            }

            Context.MoveToTarget();
        }
    }
}