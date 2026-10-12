namespace Actors.Boss.States
{
    public sealed class BossDeadState : BossState
    {
        public BossDeadState(BossContext context, StateMachine.StateMachine stateMachine) : base(context, stateMachine)
        {
        }

        public override void Enter()
        {
            Context.Stop();
        }

        public override void Tick()
        {
        }
    }
}