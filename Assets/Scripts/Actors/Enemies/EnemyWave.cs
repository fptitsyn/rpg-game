using System;
using System.Collections.Generic;

namespace Actors.Enemies
{
    public class EnemyWave
    {
        private int _enemiesCount;
        private bool _active;

        public int EnemiesCount => _enemiesCount;

        public event Action AllEnemiesDead;

        public void Add(Combatant enemy)
        {
            _enemiesCount++;
            enemy.Health.Died += OnEnemyDied;
        }
        
        public void Add(List<Combatant> enemies)
        {
            foreach (Combatant enemy in enemies)
                Add(enemy);
        }
        
        public void Begin()
        {
            _active = true;

            if (_enemiesCount == 0)
                EnemiesCleared();
        }

        private void OnEnemyDied()
        {
            _enemiesCount--;

            if (_active && _enemiesCount == 0)
                EnemiesCleared();
        }

        private void EnemiesCleared()
        {
            _active = false;
            AllEnemiesDead?.Invoke();
        }
    }
}