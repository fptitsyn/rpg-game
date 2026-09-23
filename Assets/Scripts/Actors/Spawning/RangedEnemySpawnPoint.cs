using Actors.Enemies;
using Combat.Weapons;
using UnityEngine;

namespace Actors.Spawning
{
    public sealed class RangedEnemySpawnPoint : MonoBehaviour
    {
        [SerializeField] CharacterDefinition characterDefinition;
        [SerializeField] RangedAttackDefinition[] attacks;

        public Combatant Spawn(ActorFactory factory, Combatant target, EnemyMode mode)
        {
            RangedAttackDefinition attack = attacks[Random.Range(0, attacks.Length)];
            return factory.CreateRangedEnemy(characterDefinition, transform.position, target, attack, mode);
        }
    }
}