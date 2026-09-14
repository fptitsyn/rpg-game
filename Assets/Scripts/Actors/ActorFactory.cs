using Actors.Animations;
using Actors.Enemies;
using Actors.Health;
using Actors.Player;
using CameraScripts;
using Combat;
using Combat.Projectiles;
using Game;
using UnityEngine;
using UnityEngine.AI;

namespace Actors
{
    public sealed class ActorFactory
    {
        private readonly ProjectileFactory _projectiles;
        private readonly UiFactory _ui;
        private readonly Camera _camera;

        public ActorFactory(ProjectileFactory projectiles, UiFactory ui, Camera camera)
        {
            _projectiles = projectiles;
            _ui = ui;
            _camera = camera;
        }

        public Combatant CreatePlayer(CharacterDefinition definition, Vector3 position, IPlayerInput input, OrbitCamera orbit)
        {
            var actor = Create("Player", definition, position, Faction.Player, out var combat, out var animation);
            var controller = actor.gameObject.AddComponent<CharacterController>();
            controller.height = 1.8f; controller.center = Vector3.up * 0.9f; controller.radius = 0.32f;
            controller.stepOffset = 0.3f; controller.skinWidth = 0.03f;
            var motor = actor.gameObject.AddComponent<PlayerMotor>();
            motor.Initialize(controller, combat, animation);
            actor.gameObject.AddComponent<PlayerBrain>().Initialize(input, motor, combat, actor, definition, orbit);
            orbit.Initialize(actor.transform);
            _ui.CreateHud(actor, combat);
            return actor;
        }
        
        public Combatant CreateEnemy(CharacterDefinition definition, Vector3 position, Combatant target, bool ranged)
        {
            var actor = Create(ranged ? "Ranged enemy" : "Melee enemy", definition, position, Faction.Enemy,
                out var combat, out var animation);
            var capsule = actor.gameObject.AddComponent<CapsuleCollider>();
            capsule.height = 1.8f; capsule.center = Vector3.up * 0.9f; capsule.radius = 0.35f;
            var agent = actor.gameObject.AddComponent<NavMeshAgent>();
            agent.height = 1.8f; agent.radius = 0.4f; agent.speed = definition.walkSpeed;
            agent.acceleration = 18; agent.angularSpeed = 600;
            actor.gameObject.AddComponent<EnemyBrain>().Initialize(actor, target, agent, combat, animation, ranged, definition.meleeReach);
            return actor;
        }
        private Combatant Create(string name, CharacterDefinition definition, Vector3 position, Faction faction,
            out ActorCombat combat, out CharacterAnimation animation)
        {
            var root = new GameObject(name);
            root.layer = CombatPhysics.ActorLayer;
            root.transform.position = position;
            
            var actor = root.AddComponent<Combatant>();
            actor.Initialize(definition.health, faction);
            var visual = Object.Instantiate(definition.visualPrefab, root.transform);
            visual.name = "Visual";
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localRotation = Quaternion.Euler(0, definition.visualYaw, 0);
            visual.transform.localScale = Vector3.one * definition.visualScale;
            var animator = visual.GetComponentInChildren<Animator>();
            animator.runtimeAnimatorController = definition.controller;
            animation = root.AddComponent<CharacterAnimation>();
            animation.Initialize(animator, definition);
            combat = root.AddComponent<ActorCombat>();
            combat.Initialize(actor, definition, animation,
                new MeleeAttack(actor, definition), new MagicAttack(actor, definition, _projectiles));
            _ui.CreateWorldBar(actor, _camera);
            return actor;
        }
    }
}
