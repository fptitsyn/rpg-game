using Actors.Animations;
using StateMachine;
using UnityEngine;

namespace Actors.Boss.States
{
    public sealed class BossStrongAttackState : BossState
    {
        private bool _attackStarted;
        private AttackAnimation _attackAnimation;

        public BossStrongAttackState(BossContext context, StateMachine.StateMachine stateMachine)
            : base(context, stateMachine)
        {
        }

        public override void Enter()
        {
            Context.Stop();
            Context.FaceTargetImmediately();

            AttackAnimation animation = Random.value < 0.5f
                ? AttackAnimation.Attack1
                : AttackAnimation.Attack2;

            Context.Combat.TryAttack(true, animation, Context.AttackSpeedMultiplier);
        }

        public override void Tick()
        {
            if (Context.DistanceToTarget > Context.Definition.bossStrongAttackRange ||
                !Context.HasLineOfSight)
            {
                StateMachine.Change<BossChaseState>();
                return;
            }

            Context.FaceTarget();

            if (!_attackStarted)
            {
                _attackStarted = Context.Combat.TryAttack(true, _attackAnimation,
                    Context.AttackSpeedMultiplier);

                if (_attackStarted)
                    Context.StartStrongAttackCooldown();

                return;
            }

            if (!Context.Combat.IsLocked)
                StateMachine.Change<BossRepositionState>();
        }
    }
}