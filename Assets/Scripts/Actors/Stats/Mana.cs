using System;
using UnityEngine;

namespace Actors.Stats
{
    public sealed class Mana
    {
        public float Maximum { get; }
        public float Current { get; private set; }
        
        public event Action Changed;

        public Mana(float maximum)
        {
            Maximum = maximum;
            Current = maximum;
        }

        public bool TrySpend(float amount)
        {
            if (Current < amount) return false;

            Current -= amount;
            Changed?.Invoke();
            return true;
        }

        public void Regenerate(float amount)
        {
            if (Current >= Maximum)
                return;

            Current = Math.Min(Maximum, Current + amount);
            Changed?.Invoke();
        }
        
        public void Restore(float value)
        {
            Current = Mathf.Clamp(value, 0, Maximum);
            Changed?.Invoke();
        }
    }
}