using Actors.Enemies;
using Combat;
using Combat.Weapons;
using UnityEngine;

namespace Actors.Spawning
{
    public sealed class BossSpawnPoint : MonoBehaviour
    {
        [SerializeField] CharacterDefinition characterDefinition;
        [SerializeField] WeaponDefinition[] weapons;
        [SerializeField] ElementDefinition[] elements;

        public Combatant Spawn(ActorFactory factory, Combatant target, EnemyMode mode)
        {
            WeaponDefinition weapon = weapons[Random.Range(0, weapons.Length)];
            ElementDefinition element = elements[Random.Range(0, elements.Length)];

            return factory.CreateBoss(characterDefinition, transform.position, target, weapon, element, mode);
        }
    }
}