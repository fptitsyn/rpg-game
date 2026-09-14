using System;

namespace Combat
{
    public interface IAttackEffect { void Execute(); }

    public sealed class AttackTimeline
    {
        private float _elapsed;
        private float _impactTime;
        private float _duration;
        private bool _committed;
        private IAttackEffect _effect;
        public bool IsRunning { get; private set; }

        public bool TryStart(float impact, float length, IAttackEffect attackEffect)
        {
            if (IsRunning) return false;
            if (float.IsNaN(impact) || float.IsInfinity(impact) || impact < 0 ||
                float.IsNaN(length) || float.IsInfinity(length) || length <= 0 || impact > length)
            {
                throw new ArgumentOutOfRangeException(nameof(impact));
            }
            
            _effect = attackEffect ?? throw new ArgumentNullException(nameof(attackEffect));
            _elapsed = 0;
            _impactTime = impact;
            _duration = length;
            _committed = false;
            IsRunning = true;
            return true;
        }

        public void Tick(float deltaTime)
        {
            if (float.IsNaN(deltaTime) || float.IsInfinity(deltaTime) || deltaTime < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(deltaTime));
            }
            
            if (!IsRunning) return;
            
            _elapsed += deltaTime;
            if (!_committed && _elapsed >= _impactTime)
            {
                _committed = true;
                _effect.Execute();
            }
            
            if (_elapsed >= _duration)
            {
                Cancel();
            }
        }

        public void Cancel()
        {
            IsRunning = false;
            _effect = null;
        }
    }
}
