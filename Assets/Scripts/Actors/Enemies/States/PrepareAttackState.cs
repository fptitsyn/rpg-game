using UnityEngine;

namespace Actors.Enemies.States
{
    public sealed class EnemyPrepareAttackState : EnemyState
    {
        private float _endTime;

        public EnemyPrepareAttackState(EnemyContext context, StateMachine.StateMachine stateMachine)
            : base(context, stateMachine)
        {
        }

        public override void Enter()
        {
            Context.Stop();
            Context.FaceTargetImmediately();
            float duration = Context.PreparationDuration;
            _endTime = Time.time + duration;

            Context.Animation.PlayAction("PrepareAttack", duration);
        }

        public override void Tick()
        {
            Context.FaceTarget();

            if (!Context.CanAttack())
            {
                Context.Animation.ResumeLocomotion();
                StateMachine.Change<EnemyAggressionState>();
                return;
            }

            if (Time.time >= _endTime)
                StateMachine.Change<EnemyAttackState>();
        }
    }
}