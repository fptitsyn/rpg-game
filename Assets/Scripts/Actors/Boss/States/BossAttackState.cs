using StateMachine;

namespace Actors.Boss.States
{
    public sealed class BossAttackState : BossState
    {
        private bool _attackStarted;

        public BossAttackState(BossContext context, StateMachine.StateMachine stateMachine) : base(context, stateMachine)
        {
        }

        public override void Enter()
        {
            _attackStarted = false;
            Context.Stop();
        }

        public override void Tick()
        {
            if (Context.DistanceToTarget > Context.Definition.meleeReach || !Context.HasLineOfSight)
            {
                StateMachine.Change<BossChaseState>();
                return;
            }

            Context.FaceTarget();

            if (!_attackStarted)
            {
                _attackStarted = Context.Combat.TryAttack(false, Context.AttackSpeedMultiplier);
                return;
            }

            if (!Context.Combat.IsLocked)
                StateMachine.Change<BossRepositionState>();
        }
    }
}