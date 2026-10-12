using System;
using System.Collections.Generic;
using Actors.Stats;

namespace Actors.Enemies
{
    public class EnemyKillCounter : IDisposable
    {
        private readonly List<Health> _trackedEnemies = new();

        public int KilledCount { get; private set; }

        public event Action<int> EnemyKilled;

        public void Register(Combatant enemy)
        {
            Health health = enemy.Health;
            _trackedEnemies.Add(health);
            health.Died += OnEnemyDied;
        }

        private void OnEnemyDied()
        {
            KilledCount++;
            EnemyKilled?.Invoke(KilledCount);
        }

        public void Dispose()
        {
            foreach (Health health in _trackedEnemies)
            {
                health.Died -= OnEnemyDied;
            }
        }
    }
}