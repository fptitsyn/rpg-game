using System;

namespace Actors.Stats
{
    public sealed class Health : IHealth, IDamageReceiver
    {
        public float Maximum { get; }
        public float Current { get; private set; }
        public bool IsAlive => Current > 0;
        public event Action<float, float> Changed;
        public event Action Died;
        public event Action<Damage> Damaged;

        public Health(float maximum)
        {
            if (float.IsNaN(maximum) || float.IsInfinity(maximum) || maximum <= 0)
                throw new ArgumentOutOfRangeException(nameof(maximum));
            Maximum = Current = maximum;
        }

        public void Receive(in Damage damage)
        {
            if (!IsAlive || damage.Amount <= 0) return;
            Current = Math.Max(0, Current - damage.Amount);
            Changed?.Invoke(Current, Maximum);
            
            if (IsAlive) Damaged?.Invoke(damage);
            else Died?.Invoke();
        }
        
        public void Restore(float value)
        {
            bool wasAlive = IsAlive;
            Current = Math.Clamp(value, 0f, Maximum);
            Changed?.Invoke(Current, Maximum);

            if (wasAlive && !IsAlive) Died?.Invoke();
        }
    }
}
