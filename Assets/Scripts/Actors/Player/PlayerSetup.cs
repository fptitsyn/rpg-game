using System;
using Actors.Animations;
using Actors.Health;
using CameraScripts;
using Combat;
using Combat.Projectiles;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Actors.Player
{
    public sealed class PlayerSetup : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] CharacterDefinition definition;

        [Header("Player components")]
        [SerializeField] Combatant combatant;
        [SerializeField] CharacterController characterController;
        [SerializeField] CharacterAnimation characterAnimation;
        [SerializeField] ActorCombat actorCombat;
        [SerializeField] PlayerMotor playerMotor;
        [SerializeField] PlayerBrain playerBrain;

        [Header("Visual")]
        [SerializeField] Animator animator;

        public Combatant Combatant => combatant;
        public ActorCombat Combat => actorCombat;

        public void Initialize(InputActionMap inputActions, ProjectileFactory projectileFactory, OrbitCamera orbitCamera)
        {
            gameObject.layer = CombatPhysics.ActorLayer;
            combatant.Initialize(definition.health, Faction.Player);

            animator.runtimeAnimatorController = definition.controller;
            characterAnimation.Initialize(animator, definition);

            IAttackEffect meleeAttack = new MeleeAttack(combatant, definition);
            IAttackEffect magicAttack = new MagicAttack(combatant, definition, projectileFactory);

            actorCombat.Initialize(combatant, definition, characterAnimation, meleeAttack, magicAttack);
            playerMotor.Initialize(characterController, actorCombat, characterAnimation);
            playerBrain.Initialize(inputActions, playerMotor, actorCombat, combatant, definition, orbitCamera);
            orbitCamera.Initialize(transform);
        }
    }
}