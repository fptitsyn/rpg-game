using Actors.Animations;
using Actors.Boss;
using Actors.Enemies;
using Actors.Stats;
using Actors.Visual;
using Combat;
using Combat.Projectiles;
using Combat.Weapons;
using UI.InGame;
using UnityEngine;

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

        public Combatant CreateMeleeEnemy(CharacterDefinition definition, Vector3 position, Combatant target,
            MeleeAttackDefinition attackDefinition, EnemyMode enemyMode)
        {
            Combatant actor = Create(definition, position, Faction.Enemy, out ActorCombat combat,
                out CharacterAnimation animation);

            IAttackPresentation presentation = null;

            if (attackDefinition.source == MeleeAttackSource.Weapon)
                presentation = EquipWeapon(actor, attackDefinition.weapon, attackDefinition.weapon.defaultEffect);

            IAttackEffect attack = new MeleeAttack(actor, definition, attackDefinition.DamageMultiplier,
                attackDefinition.RangeMultiplier, presentation);

            combat.Initialize(actor, definition, animation, attack, attack);

            EnemyBrain enemyBrain = actor.gameObject.AddComponent<EnemyBrain>();
            enemyBrain.Initialize(actor, target, combat, animation, definition, enemyMode, false,
                attackDefinition.AnimationVariant, false, 0f);

            return actor;
        }

        public Combatant CreateRangedEnemy(CharacterDefinition definition, Vector3 position, Combatant target,
            RangedAttackDefinition attackDefinition, EnemyMode enemyMode)
        {
            Combatant actor = Create(definition, position, Faction.Enemy, out ActorCombat combat,
                out CharacterAnimation animation);

            CharacterMaterialView materialView = actor.GetComponentInChildren<CharacterMaterialView>();
            materialView.Apply(attackDefinition.characterMaterial);

            IAttackEffect attack = new MagicAttack(actor, definition, _projectiles,
                attackDefinition.damageMultiplier, attackDefinition.projectileSpeedMultiplier,
                attackDefinition.projectileColor);

            combat.Initialize(actor, definition, animation, attack, attack);

            bool requiresPreparation = attackDefinition.animationVariant == AttackAnimation.Attack2;

            EnemyBrain enemyBrain = actor.gameObject.AddComponent<EnemyBrain>();
            enemyBrain.Initialize(actor, target, combat, animation, definition, enemyMode, true,
                attackDefinition.animationVariant, requiresPreparation, attackDefinition.preparationDuration);

            return actor;
        }

        public Combatant CreateBoss(CharacterDefinition definition, Vector3 position, Combatant target,
            WeaponDefinition meleeWeapon, ElementDefinition element, EnemyMode enemyMode)
        {
            Combatant actor = Create(
                definition,
                position,
                Faction.Enemy,
                out ActorCombat combat,
                out CharacterAnimation animation
            );

            WeaponEffectDefinition meleeEffect = element.meleeEffect;
            WeaponEffectDefinition rangedEffect = element.rangedEffect;

            WeaponView weaponView = EquipWeapon(actor, meleeWeapon, meleeEffect);

            IAttackEffect meleeAttack = new MeleeAttack(
                actor,
                definition,
                meleeWeapon.damageMultiplier * element.damageMultiplier,
                meleeWeapon.rangeMultiplier,
                weaponView
            );

            IAttackEffect magicAttack = new MagicAttack(
                actor,
                definition,
                _projectiles,
                element.damageMultiplier,
                1f,
                null,
                rangedEffect.projectileEffect
            );

            combat.Initialize(actor, definition, animation, meleeAttack, magicAttack);

            BossBrain bossBrain = actor.gameObject.AddComponent<BossBrain>();
            bossBrain.Initialize(actor, target, combat, animation, definition, enemyMode);

            return actor;
        }

        private Combatant Create(CharacterDefinition definition, Vector3 position, Faction faction,
            out ActorCombat combat, out CharacterAnimation animation)
        {
            GameObject root = Object.Instantiate(definition.visualPrefab, position, Quaternion.identity);

            Combatant actor = root.AddComponent<Combatant>();
            actor.Initialize(definition.health, faction);

            Animator animator = root.GetComponentInChildren<Animator>();
            animator.runtimeAnimatorController = definition.controller;

            animation = root.AddComponent<CharacterAnimation>();
            animation.Initialize(animator, definition);

            combat = root.AddComponent<ActorCombat>();

            HealthBar healthBar = root.GetComponentInChildren<HealthBar>(true);

            if (healthBar)
                healthBar.Bind(actor.Health, _camera);

            return actor;
        }

        private WeaponView EquipWeapon(Combatant actor, WeaponDefinition weapon, WeaponEffectDefinition effect)
        {
            WeaponSocket socket = actor.GetComponentInChildren<WeaponSocket>();
            GameObject weaponObject = Object.Instantiate(weapon.visualPrefab, socket.transform);

            weaponObject.transform.localPosition = Vector3.zero;
            weaponObject.transform.localRotation = Quaternion.identity;

            WeaponView weaponView = weaponObject.GetComponent<WeaponView>();
            weaponView.Initialize(effect);

            return weaponView;
        }
    }
}