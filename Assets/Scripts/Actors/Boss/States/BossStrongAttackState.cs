using StateMachine;

namespace Actors.Boss.States
{
    public sealed class BossStrongAttackState : BossState
    {
        private bool _attackStarted;

        public BossStrongAttackState(BossContext context, StateMachine.StateMachine stateMachine) : base(context, stateMachine)
        {
        }

        public override void Enter()
        {
            _attackStarted = false;
            Context.Stop();
        }

        public override void Tick()
        {
            if (Context.DistanceToTarget > Context.Definition.bossStrongAttackRange || !Context.HasLineOfSight)
            {
                StateMachine.Change<BossChaseState>();
                return;
            }

            Context.FaceTarget();

            if (!_attackStarted)
            {
                _attackStarted = Context.Combat.TryAttack(true, Context.AttackSpeedMultiplier);

                if (_attackStarted)
                    Context.StartStrongAttackCooldown();

                return;
            }

            if (!Context.Combat.IsLocked)
                StateMachine.Change<BossRepositionState>();
        }
    }
}