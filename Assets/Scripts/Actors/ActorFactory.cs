using Actors.Animations;
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

        public Combatant CreateEnemy(CharacterDefinition definition, Vector3 position, Combatant target, bool ranged)
        {
            Combatant actor = Create(
                ranged ? "Ranged enemy" : "Melee enemy",
                definition,
                position,
                Faction.Enemy,
                out ActorCombat combat,
                out CharacterAnimation animation);

            CapsuleCollider capsule = actor.gameObject.AddComponent<CapsuleCollider>();

            capsule.height = 1.8f;
            capsule.center = Vector3.up * 0.9f;
            capsule.radius = 0.35f;

            NavMeshAgent agent = actor.gameObject.AddComponent<NavMeshAgent>();

            agent.height = 1.8f;
            agent.radius = 0.4f;
            agent.speed = definition.walkSpeed;
            agent.acceleration = 18;
            agent.angularSpeed = 600;

            EnemyBrain brain = actor.gameObject.AddComponent<EnemyBrain>();
            brain.Initialize(actor, target, agent, combat, animation, ranged, definition.meleeReach);
            
            return actor;
        }

        private Combatant Create(
            string objectName, CharacterDefinition definition, Vector3 position,
            Faction faction, out ActorCombat combat, out CharacterAnimation animation)
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
    }
}