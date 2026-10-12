using System.Collections.Generic;
using Actors;
using Actors.Stats;
using Combat.Projectiles;
using Combat.Weapons;
using UnityEngine;

namespace Combat
{
    public static class CombatPhysics
    {
        public const int WorldLayer = 8;
        public const int ActorLayer = 9;
        public const int WorldMask = 1 << WorldLayer;
        public const int CombatMask = WorldMask | (1 << ActorLayer);

        public static bool ClearLine(Vector3 from, Vector3 to) =>
            !Physics.Linecast(from, to, WorldMask, QueryTriggerInteraction.Ignore);
    }

    public sealed class MeleeAttack : IAttackEffect
    {
        private readonly Combatant _owner;
        private readonly CharacterDefinition _definition;
        private readonly float _damageMultiplier;
        private readonly float _rangeMultiplier;
        private readonly IAttackPresentation _presentation;

        public MeleeAttack(Combatant owner, CharacterDefinition definition, float damageMultiplier = 1f,
            float rangeMultiplier = 1f, IAttackPresentation presentation = null)
        {
            _owner = owner;
            _definition = definition;
            _damageMultiplier = damageMultiplier;
            _rangeMultiplier = rangeMultiplier;
            _presentation = presentation;
        }

        public void Execute()
        {
            if (!_owner.IsAlive)
                return;

            _presentation?.Play();

            float reach = _definition.meleeReach * _rangeMultiplier;
            HashSet<Combatant> struck = new();

            foreach (Collider targetCollider in Physics.OverlapSphere(_owner.AimPoint, reach,
                         1 << CombatPhysics.ActorLayer, QueryTriggerInteraction.Ignore))
            {
                Combatant target = targetCollider.GetComponentInParent<Combatant>();

                if (!target || !target.IsAlive || target.Faction == _owner.Faction || !struck.Add(target))
                    continue;

                Vector3 direction = target.transform.position - _owner.transform.position;
                direction.y = 0f;

                if (direction.magnitude > reach)
                    continue;

                if (Vector3.Angle(_owner.transform.forward, direction) > _definition.meleeArc * 0.5f)
                    continue;

                if (!CombatPhysics.ClearLine(_owner.AimPoint, target.AimPoint))
                    continue;

                float damageAmount = _definition.physicalDamage * _damageMultiplier;
                Damage damage = new(damageAmount, DamageType.Physical, _owner.Faction);
                target.Receive(damage);
            }
        }
    }

    public sealed class MagicAttack : IAttackEffect
    {
        private readonly Combatant _owner;
        private readonly CharacterDefinition _definition;
        private readonly ProjectileFactory _projectiles;
        private readonly float _damageMultiplier;
        private readonly float _speedMultiplier;
        private readonly Color _projectileColor;
        private readonly GameObject _projectileEffect;
        private readonly IAttackPresentation _presentation;

        public MagicAttack(Combatant owner, CharacterDefinition definition, ProjectileFactory projectiles,
            float damageMultiplier = 1f, float speedMultiplier = 1f, Color? projectileColor = null,
            GameObject projectileEffect = null, IAttackPresentation presentation = null)
        {
            _owner = owner;
            _definition = definition;
            _projectiles = projectiles;
            _damageMultiplier = damageMultiplier;
            _speedMultiplier = speedMultiplier;
            _projectileColor = projectileColor ?? definition.projectileColor;
            _projectileEffect = projectileEffect;
            _presentation = presentation;
        }

        public void Execute()
        {
            if (!_owner.IsAlive)
                return;

            _presentation?.Play();

            float damage = _definition.magicalDamage * _damageMultiplier;
            float speed = _definition.projectileSpeed * _speedMultiplier;
            Vector3 position = _presentation?.AttackPoint ?? _owner.AimPoint;

            _projectiles.Spawn(_owner, _definition, position, _owner.transform.forward,
                damage, speed, _projectileColor, _projectileEffect);
        }
    }
}
