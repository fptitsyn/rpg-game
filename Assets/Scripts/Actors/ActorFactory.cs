using Actors.Animations;
using Actors.Boss;
using Actors.Enemies;
using Actors.Stats;
using Combat;
using Combat.Projectiles;
using UI;
using UI.InGame;
using UnityEngine;
using UnityEngine.AI;

namespace Actors
{
    public sealed class ActorFactory
    {
        private readonly ProjectileFactory _projectiles;
        private readonly Camera _camera;

        public ActorFactory(ProjectileFactory projectiles, Camera camera)
        {
            _projectiles = projectiles;
            _camera = camera;
        }

        public Combatant CreateEnemy(CharacterDefinition definition, Vector3 position, Combatant target, bool ranged,
            EnemyMode enemyMode)
        {
            Combatant actor = Create(
                definition,
                position,
                Faction.Enemy,
                out ActorCombat combat,
                out CharacterAnimation animation
            );

            EnemyBrain enemyBrain = actor.gameObject.AddComponent<EnemyBrain>();
            enemyBrain.Initialize(actor, target, combat, animation, definition, enemyMode, ranged);
            
            return actor;
        }

        private Combatant Create(CharacterDefinition definition, Vector3 position,
            Faction faction, out ActorCombat combat, out CharacterAnimation animation
        )
        {
            // GameObject root = new GameObject(objectName);
            //
            // root.layer = CombatPhysics.ActorLayer;
            // root.transform.position = position;
            GameObject root = Object.Instantiate(definition.visualPrefab, position, Quaternion.identity);
            Combatant actor = root.AddComponent<Combatant>();
            actor.Initialize(definition.health, faction);
            
            Animator animator = root.GetComponentInChildren<Animator>();
            animator.runtimeAnimatorController = definition.controller;
            animation = root.AddComponent<CharacterAnimation>();
            animation.Initialize(animator, definition);

            combat = root.AddComponent<ActorCombat>();

            combat.Initialize(actor, definition, animation, new MeleeAttack(actor, definition),
                new MagicAttack(actor, definition, _projectiles));

            HealthBar healthBar = root.GetComponentInChildren<HealthBar>(true);

            if (healthBar)
            {
                healthBar.Bind(actor.Health, _camera);
            }

            return actor;
        }
        
        public Combatant CreateBoss(CharacterDefinition definition, Vector3 position, Combatant target, EnemyMode enemyMode)
        {
            Combatant actor = Create(definition, position, Faction.Enemy, out ActorCombat combat,
                out CharacterAnimation animation);
        
            BossBrain bossBrain = actor.gameObject.AddComponent<BossBrain>();
            bossBrain.Initialize(actor, target, combat, animation, definition, enemyMode);
        
            return actor;
        }
    }
}