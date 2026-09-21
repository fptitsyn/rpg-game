namespace Actors.Boss.States
{
    public sealed class BossIdleState : BossState
    {
        public BossIdleState(BossContext context, StateMachine.StateMachine stateMachine) : base(context, stateMachine)
        {
        }

        public override void Enter()
        {
            Context.Stop();
        }

        public override void Tick()
        {
            if (Context.ShouldBossAggro)
                StateMachine.Change<BossAggressionState>();
        }
    }
}