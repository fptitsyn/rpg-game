using StateMachine;

namespace Actors.Enemies.States
{
    public sealed class EnemyAttackState : EnemyState
    {
        private bool _attackStarted;

        public EnemyAttackState(EnemyContext context, StateMachine.StateMachine stateMachine) : base(context, stateMachine)
        {
        }

        public override void Enter()
        {
            _attackStarted = false;
            Context.Stop();
            Context.FaceTargetImmediately();
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

            if (!Context.CanAttack())
            {
                StateMachine.Change<EnemyAggressionState>();
                return;
            }

            Context.FaceTarget();

            if (!_attackStarted)
            {
                _attackStarted = Context.Combat.TryAttack(Context.Ranged, Context.SelectedAttackAnimation);
                return;
            }

            if (!Context.Combat.IsLocked)
                StateMachine.Change<EnemyAggressionState>();
        }
    }
}