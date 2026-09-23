using Actors.Enemies;
using Combat.Weapons;
using UnityEngine;

namespace Actors.Spawning
{
    public sealed class MeleeEnemySpawnPoint : MonoBehaviour
    {
        [SerializeField] CharacterDefinition characterDefinition;
        [SerializeField] MeleeAttackDefinition[] attacks;

        public Combatant Spawn(ActorFactory factory, Combatant target, EnemyMode mode)
        {
            MeleeAttackDefinition attack = attacks[Random.Range(0, attacks.Length)];
            return factory.CreateMeleeEnemy(characterDefinition, transform.position, target, attack, mode);
        }
    }
}