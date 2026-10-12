using Actors.Animations;
using Actors.Stats;
using CameraScripts;
using Combat;
using Combat.Projectiles;
using SaveSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Actors.Player
{
    public sealed class PlayerSetup : MonoBehaviour, IPlayerSaveState
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
            combatant.Initialize(definition.health, Faction.Player, definition.maxMana);

            animator.runtimeAnimatorController = definition.controller;
            characterAnimation.Initialize(animator, definition);

            IAttackEffect meleeAttack = new MeleeAttack(combatant, definition);
            IAttackEffect magicAttack = new MagicAttack(combatant, definition, projectileFactory);

            actorCombat.Initialize(combatant, definition, characterAnimation, meleeAttack, magicAttack);
            playerMotor.Initialize(characterController, actorCombat, characterAnimation);
            playerBrain.Initialize(inputActions, playerMotor, actorCombat, combatant, definition, orbitCamera);
            orbitCamera.Initialize(transform);
        }

        public SaveData.PlayerSaveData Capture()
        {
            return new SaveData.PlayerSaveData
            {
                position = transform.position,
                rotation = transform.rotation,
                health = combatant.Health.Current,
                mana = combatant.Mana.Current
            };
        }

        public void Restore(SaveData.PlayerSaveData data)
        {
            characterController.enabled = false;
            transform.SetPositionAndRotation(data.position, data.rotation);
            characterController.enabled = true;

            playerMotor.ResetMovement();
            combatant.Health.Restore(data.health);
            combatant.Mana.Restore(data.mana);
            actorCombat.ResetAfterLoad();
        }
    }
}