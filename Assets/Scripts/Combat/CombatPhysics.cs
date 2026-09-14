using System.Collections.Generic;
using Actors;
using Actors.Health;
using Combat.Projectiles;
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
        private readonly CharacterDefinition _settings;

        public MeleeAttack(Combatant owner, CharacterDefinition settings)
        {
            _owner = owner; 
            _settings = settings;
        }

        public void Execute()
        {
            if (!_owner.IsAlive) return;
            var struck = new HashSet<Combatant>();
            foreach (var collider in Physics.OverlapSphere(_owner.AimPoint, _settings.meleeReach,
                1 << CombatPhysics.ActorLayer, QueryTriggerInteraction.Ignore))
            {
                var target = collider.GetComponentInParent<Combatant>();
                if (!target || !target.IsAlive || target.Faction == _owner.Faction || !struck.Add(target)) continue;
                Vector3 direction = target.transform.position - _owner.transform.position;
                direction.y = 0;
                if (direction.magnitude > _settings.meleeReach ||
                    Vector3.Angle(_owner.transform.forward, direction) > _settings.meleeArc * 0.5f ||
                    !CombatPhysics.ClearLine(_owner.AimPoint, target.AimPoint)) continue;
                IDamageReceiver receiver = target;
                receiver.Receive(new Damage(_settings.physicalDamage, DamageType.Physical, _owner.Faction));
            }
        }
    }

    public sealed class MagicAttack : IAttackEffect
    {
        private readonly Combatant _owner;
        private readonly CharacterDefinition _settings;
        private readonly ProjectileFactory _projectiles;

        public MagicAttack(Combatant owner, CharacterDefinition settings, ProjectileFactory projectiles)
        {
            _owner = owner; 
            _settings = settings; 
            _projectiles = projectiles;
        }
        
        public void Execute()
        {
            if (_owner.IsAlive) _projectiles.Spawn(_owner, _settings, _owner.transform.forward);
        }
    }
}
