using Actors.Animations;
using Actors.Enemies;
using Combat;
using UnityEngine;
using UnityEngine.AI;

namespace Actors.Boss
{
    public sealed class BossContext : EnemyContext
    {
        private float _nextStrongAttack;

        public float AttackSpeedMultiplier
        {
            get
            {
                if (HealthRatio <= Definition.enragedHealthRatio)
                    return Definition.enragedAttackSpeed;

                return 1f;
            }
        }

        public bool StrongAttackReady => Time.time >= _nextStrongAttack;

        public bool ShouldBossAggro
        {
            get
            {
                if (Mode == EnemyMode.Peaceful)
                    return WasAttacked;

                return WasAttacked || DistanceToTarget <= Definition.detectionDistance;
            }
        }

        public BossContext(Transform transform, Combatant actor, Combatant target, NavMeshAgent agent,
            ActorCombat combat, ICharacterAnimation anim, CharacterDefinition definition, EnemyMode mode)
            : base(transform, actor, target, agent, combat, anim, definition, mode, false)
        {
        }

        public bool CanUseStrongAttack()
        {
            return StrongAttackReady && DistanceToTarget <= Definition.bossStrongAttackRange && HasLineOfSight;
        }

        public void StartStrongAttackCooldown()
        {
            _nextStrongAttack = Time.time + Definition.bossStrongAttackCooldown / AttackSpeedMultiplier;
        }
    }
}