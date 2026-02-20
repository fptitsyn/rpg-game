namespace Actors.Enemies
{
    public class Enemy : Actor
    {
        protected override void ReceiveDamage(float damage)
        {
            base.ReceiveDamage(damage);

            if (CurrentHealth <= 0)
            {
                // event
            }
        }
    }
}
