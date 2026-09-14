using System;

namespace Actors.Health
{
    public enum DamageType { Physical, Magical }
    public enum Faction { Player, Enemy }

    public readonly struct Damage
    {
        public float Amount { get; }
        public DamageType Type { get; }
        public Faction Source { get; }

        public Damage(float amount, DamageType type, Faction source)
        {
            if (float.IsNaN(amount) || float.IsInfinity(amount) || amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount));
            Amount = amount;
            Type = type;
            Source = source;
        }
    }

    public interface IDamageReceiver
    {
        bool IsAlive { get; }
        void Receive(in Damage damage);
    }

    public interface IHealth
    {
        float Current { get; }
        float Maximum { get; }
        event Action<float, float> Changed;
        event Action Died;
    }
}
