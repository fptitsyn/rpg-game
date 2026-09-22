using System.Collections.Generic;
using Combat;
using UnityEngine;
using UnityEngine.AI;

namespace Actors.Enemies
{
    public sealed class EnemySpawner
    {
        private readonly ActorFactory _factory;
        private readonly List<Vector3> _occupied = new();
        private readonly EnemyMode _enemyMode;
        
        public EnemySpawner(ActorFactory factory, EnemyMode enemyMode = EnemyMode.Normal)
        {
            _factory = factory;
            _enemyMode = enemyMode;
        }
        
        public List<Combatant> Spawn(CharacterDefinition settings, Combatant player, bool ranged, int count)
        {
            List<Combatant> spawnedEnemies = new();
            
            for (int i = 0; i < count; i++)
            {
                bool spawned = false;
                for (int attempt = 0; attempt < 150; attempt++)
                {
                    Vector3 point = new Vector3(Random.Range(-21f, 21f), 0, Random.Range(-21f, 21f));
                    if (!NavMesh.SamplePosition(point, out var hit, 1, NavMesh.AllAreas))
                    {
                        continue;
                    }
                    
                    point = hit.position;
                    if (Vector3.Distance(point, player.transform.position) < 7 || _occupied.Exists(p => Vector3.Distance(p, point) < 3))
                    {
                        continue;
                    }
                    if (Physics.CheckCapsule(point + Vector3.up * 0.45f, point + Vector3.up * 1.5f,
                        0.4f, CombatPhysics.WorldMask, QueryTriggerInteraction.Ignore))
                    {
                        continue;
                    }
                    var path = new NavMeshPath();
                    if (!NavMesh.CalculatePath(point, player.transform.position, NavMesh.AllAreas, path) ||
                        path.status != NavMeshPathStatus.PathComplete)
                    {
                        continue;
                    }
                    
                    Combatant enemy = _factory.CreateEnemy(settings, point, player, ranged, _enemyMode);
                    
                    spawnedEnemies.Add(enemy);
                    _occupied.Add(point);
                    spawned = true;
                    break;
                }
                
                if (!spawned) Debug.LogError("Game: no reachable spawn point for enemy " + i);
            }

            return spawnedEnemies;
        }

        public Combatant SpawnBoss(CharacterDefinition definition, Vector3 position, Combatant player, EnemyMode mode)
        {
            return _factory.CreateBoss(definition, position, player, mode);
        }
    }
}
