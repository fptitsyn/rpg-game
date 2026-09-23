using Actors.Animations;
using StateMachine;
using UnityEngine;

namespace Actors.Boss.States
{
    public sealed class BossAttackState : BossState
    {
        private bool _attackStarted;
        private AttackAnimation _attackAnimation;

        public BossAttackState(BossContext context, StateMachine.StateMachine stateMachine) : base(context, stateMachine)
        {
        }

        public override void Enter()
        {
            Context.Stop();
            Context.FaceTargetImmediately();

            AttackAnimation animation = Random.value < 0.5f
                ? AttackAnimation.Attack1
                : AttackAnimation.Attack2;

            Context.Combat.TryAttack(false, animation, Context.AttackSpeedMultiplier);
        }

        public override void Tick()
        {
            if (!Context.CanAttack())
            {
                StateMachine.Change<BossChaseState>();
                return;
            }

            Context.FaceTarget();

            if (!_attackStarted)
            {
                _attackStarted = Context.Combat.TryAttack(false, _attackAnimation,
                    Context.AttackSpeedMultiplier);

                return;
            }

            if (!Context.Combat.IsLocked)
                StateMachine.Change<BossRepositionState>();
        }
    }
}