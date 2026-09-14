using Actors.Health;
using UnityEngine;

namespace Actors
{
    public sealed class Combatant : MonoBehaviour, IDamageReceiver
    {
        public Health.Health Health { get; private set; }
        public Faction Faction { get; private set; }
        public bool IsAlive => Health != null && Health.IsAlive;
        public Vector3 AimPoint => transform.position + Vector3.up * 1.15f;
        public void Initialize(float maximumHealth, Faction faction)
        {
            Health = new Health.Health(maximumHealth);
            Faction = faction;
        }
        public void Receive(in Damage damage)
        {
            if (damage.Source != Faction) Health?.Receive(damage);
        }
    }
}
