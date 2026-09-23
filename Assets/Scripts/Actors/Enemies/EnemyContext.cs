using Actors.Animations;
using Combat;
using UnityEngine;
using UnityEngine.AI;

namespace Actors.Enemies
{
    public class EnemyContext
    {
        private readonly NavMeshPath _path;
        private float _nextPathUpdate;

        public Transform Transform { get; }
        public Combatant Actor { get; }
        public Combatant Target { get; }
        public NavMeshAgent Agent { get; }
        public ActorCombat Combat { get; }
        public CharacterDefinition Definition { get; }
        public EnemyMode Mode { get; }
        public bool Ranged { get; }
        public bool WasAttacked { get; set; }
        
        public ICharacterAnimation Animation { get; }
        public AttackAnimation SelectedAttackAnimation { get; }
        public bool RequiresPreparation { get; }
        public float PreparationDuration { get; }

        public float DistanceToTarget
        {
            get
            {
                Vector3 offset = Target.transform.position - Transform.position;
                offset.y = 0f;
                return offset.magnitude;
            }
        }

        public float HealthRatio => Actor.Health.Current / Actor.Health.Maximum;
        public bool IsLowHealth => HealthRatio <= Definition.fleeHealthRatio;
        public bool HasLineOfSight => CombatPhysics.ClearLine(Actor.AimPoint, Target.AimPoint);

        public bool ShouldAggro
        {
            get
            {
                if (Mode == EnemyMode.Peaceful)
                    return false;

                return WasAttacked || DistanceToTarget <= Definition.detectionDistance;
            }
        }

        public EnemyContext(Transform transform, Combatant actor, Combatant target, NavMeshAgent agent,
            ActorCombat combat, ICharacterAnimation animation, CharacterDefinition definition, EnemyMode mode,
            bool ranged, AttackAnimation attackAnimation, bool requiresPreparation, float preparationDuration)
        {
            Transform = transform;
            Actor = actor;
            Target = target;
            Agent = agent;
            Combat = combat;
            Animation = animation;
            Definition = definition;
            Mode = mode;
            Ranged = ranged;
            SelectedAttackAnimation = attackAnimation;
            RequiresPreparation = requiresPreparation;
            PreparationDuration = preparationDuration;
            _path = new NavMeshPath();
        }

        protected EnemyContext(Transform transform, Combatant actor, Combatant target, NavMeshAgent agent,
            ActorCombat combat, ICharacterAnimation animation, CharacterDefinition definition, EnemyMode mode, bool ranged)
            : this(transform, actor, target, agent, combat, animation, definition, mode, ranged,
                AttackAnimation.Attack1, false, 0f)
        {
        }
        
        public bool CanAttack()
        {
            if (!HasLineOfSight)
                return false;

            if (!Ranged)
                return DistanceToTarget <= Definition.meleeReach;

            return DistanceToTarget >= Definition.rangedMinDistance && DistanceToTarget <= Definition.rangedMaxDistance;
        }

        public void Stop()
        {
            Agent.isStopped = true;

            if (Agent.hasPath)
                Agent.ResetPath();

            Agent.velocity = Vector3.zero;
        }

        public void MoveToTarget()
        {
            if (Time.time < _nextPathUpdate)
                return;

            _nextPathUpdate = Time.time + 0.2f;

            Agent.isStopped = false;
            Agent.stoppingDistance = Ranged ? Definition.rangedMaxDistance * 0.85f : Definition.meleeReach * 0.8f;

            Agent.SetDestination(Target.transform.position);
        }

        public void MoveAwayFromTarget()
        {
            if (Time.time < _nextPathUpdate)
                return;

            _nextPathUpdate = Time.time + 0.25f;

            Vector3 toTarget = Target.transform.position - Transform.position;
            toTarget.y = 0f;

            Vector3 away = toTarget.sqrMagnitude > 0.001f ? -toTarget.normalized : -Transform.forward;
            Vector3 bestDestination = Transform.position;
            float bestDistance = toTarget.sqrMagnitude;

            for (int i = -2; i <= 2; i++)
            {
                Vector3 direction = Quaternion.Euler(0f, i * 40f, 0f) * away;
                Vector3 point = Transform.position + direction * Definition.fleeDistance;

                if (!NavMesh.SamplePosition(point, out NavMeshHit hit, 1.5f, NavMesh.AllAreas))
                    continue;

                if (!Agent.CalculatePath(hit.position, _path))
                    continue;

                if (_path.status != NavMeshPathStatus.PathComplete)
                    continue;

                float distance = (hit.position - Target.transform.position).sqrMagnitude;

                if (distance <= bestDistance)
                    continue;

                bestDistance = distance;
                bestDestination = hit.position;
            }

            if (bestDestination == Transform.position)
            {
                Stop();
                return;
            }

            Agent.isStopped = false;
            Agent.stoppingDistance = 0.2f;
            Agent.SetDestination(bestDestination);
        }

        public void FaceTarget()
        {
            Vector3 direction = Target.transform.position - Transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude < 0.001f)
                return;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            Transform.rotation = Quaternion.RotateTowards(Transform.rotation, targetRotation, 720f * Time.deltaTime);
        }

        public void ForgetTarget()
        {
            WasAttacked = false;
        }
    }
}