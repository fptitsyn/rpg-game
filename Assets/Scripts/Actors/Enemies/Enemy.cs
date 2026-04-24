namespace Actors.Enemies
{
    public class Enemy : Actor
    {
        public override void ReceiveDamage(float damage)
        {
            base.ReceiveDamage(damage);

            if (CurrentHealth <= 0)
            {
                // event
            }
        }
    }
}
