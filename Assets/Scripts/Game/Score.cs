using System;

namespace Game
{
    public sealed class Score
    {
        public int Value { get; private set; }

        public event Action<int> Changed;

        public void Add(int amount)
        {
            Value += amount;
            Changed?.Invoke(Value);
        }
    }
}