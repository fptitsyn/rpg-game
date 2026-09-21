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
                ranged ? "Ranged enemy" : "Melee enemy",
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

        private Combatant Create(
            string objectName, CharacterDefinition definition, Vector3 position,
            Faction faction, out ActorCombat combat, out CharacterAnimation animation
        )
        {
            GameObject root = new GameObject(objectName);

            root.layer = CombatPhysics.ActorLayer;
            root.transform.position = position;

            Combatant actor = root.AddComponent<Combatant>();
            actor.Initialize(definition.health, faction);

            GameObject visual = Object.Instantiate(definition.visualPrefab, root.transform);

            visual.name = "Visual";
            visual.transform.localPosition = Vector3.zero;

            Animator animator = visual.GetComponentInChildren<Animator>();
            animator.runtimeAnimatorController = definition.controller;
            animation = root.AddComponent<CharacterAnimation>();
            animation.Initialize(animator, definition);

            combat = root.AddComponent<ActorCombat>();

            combat.Initialize(actor, definition, animation, new MeleeAttack(actor, definition),
                new MagicAttack(actor, definition, _projectiles));

            HealthBar healthBar = visual.GetComponentInChildren<HealthBar>(true);

            if (healthBar != null)
            {
                healthBar.Bind(actor.Health, _camera);
            }

            return actor;
        }
        
        public Combatant CreateBoss(CharacterDefinition definition, Vector3 position, Combatant target, EnemyMode enemyMode)
        {
            Combatant actor = Create("Boss", definition, position, Faction.Enemy, out ActorCombat combat,
                out CharacterAnimation animation);
        
            BossBrain bossBrain = actor.gameObject.AddComponent<BossBrain>();
            bossBrain.Initialize(actor, target, combat, animation, definition, enemyMode);
        
            return actor;
        }
    }
}