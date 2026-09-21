using UnityEngine;

namespace Actors.Boss.States
{
    public sealed class BossRepositionState : BossState
    {
        private float _endTime;

        public BossRepositionState(BossContext context, StateMachine.StateMachine stateMachine) : base(context, stateMachine)
        {
        }

        public override void Enter()
        {
            _endTime = Time.time + Context.Definition.bossRepositionDuration;
        }

        public override void Tick()
        {
            if (Time.time >= _endTime)
            {
                StateMachine.Change<BossAggressionState>();
                return;
            }

            Context.MoveAwayFromTarget();
        }

        public override void Exit()
        {
            Context.Stop();
        }
    }
}