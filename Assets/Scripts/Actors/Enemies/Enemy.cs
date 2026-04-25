namespace Actors.Enemies
{
    public class Enemy : Actor
    {
        private EnemyStateMachine stateMachine;

        protected override void Awake()
        {
            base.Awake();
            stateMachine = GetComponent<EnemyStateMachine>();
        }

        public override void ReceiveDamage(float damage)
        {
            base.ReceiveDamage(damage);
            stateMachine?.OnDamageReceived();
        }
    }
}