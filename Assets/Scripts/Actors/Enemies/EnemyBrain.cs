using Actors.Animations;
using Combat;
using UnityEngine;
using UnityEngine.AI;

namespace Actors.Enemies
{
    public sealed class EnemyBrain : MonoBehaviour
    {
        private Combatant _target;
        private Combatant _actor;
        private NavMeshAgent _agent;
        private ActorCombat _combat;
        private ICharacterAnimation _animationView;
        private bool _ranged;
        private float _reach;
        private float _nextPath;
        private NavMeshPath _candidatePath;

        private void Awake()
        {
            _candidatePath = new NavMeshPath();
        }

        public void Initialize(Combatant owner, Combatant player, NavMeshAgent navigation,
            ActorCombat attacks, ICharacterAnimation view, bool isRanged, float meleeReach)
        {
            _actor = owner;
            _target = player;
            _agent = navigation;
            _combat = attacks;
            _animationView = view;
            _ranged = isRanged;
            _reach = meleeReach;
        }
        
        private void Update()
        {
            if (!_agent || !_agent.enabled || !_agent.isOnNavMesh) return;

            if (!_actor.IsAlive || !_target || !_target.IsAlive || _combat.IsLocked)
            {
                Stop();
                _animationView.SetSpeed(0);
                return;
            }
            
            Vector3 toTarget = _target.transform.position - transform.position;
            toTarget.y = 0;
            float distance = toTarget.magnitude;
            bool clearShot = CombatPhysics.ClearLine(_actor.AimPoint, _target.AimPoint);
            
            var intent = EnemyDecision.Evaluate(distance, _target.IsAlive, _ranged, clearShot, 10, _reach);
            switch (intent)
            {
                case EnemyIntent.Idle: 
                    Stop();
                    break;
                case EnemyIntent.Approach:
                    Navigate(_target.transform.position, !clearShot ? 0.3f : _ranged ? 6.5f : _reach * 0.8f);
                    break;
                case EnemyIntent.Retreat:
                    Retreat(toTarget);
                    break;
                case EnemyIntent.Attack:
                    Stop();
                    if (toTarget.sqrMagnitude > 0.001f)
                    {
                        transform.rotation = Quaternion.RotateTowards(transform.rotation,
                            Quaternion.LookRotation(toTarget), 720 * Time.deltaTime);
                    }
                    if (Vector3.Angle(transform.forward, toTarget) < 8)
                    {
                        _combat.TryAttack(_ranged);
                    }
                    break;
            }
            
            _animationView.SetSpeed(_agent.velocity.magnitude);
        }
        
        private void Stop()
        {
            _agent.isStopped = true;
            if (_agent.hasPath) _agent.ResetPath();
            _agent.velocity = Vector3.zero;
        }
        
        private void Navigate(Vector3 destination, float stopDistance)
        {
            _agent.isStopped = false;
            _agent.stoppingDistance = stopDistance;
            
            if (Time.time < _nextPath) return;
            
            _nextPath = Time.time + 0.2f;
            _agent.SetDestination(destination);
        }
        
        private void Retreat(Vector3 toTarget)
        {
            if (Time.time < _nextPath) return;
            
            _nextPath = Time.time + 0.25f;
            Vector3 away = toTarget.sqrMagnitude > 0.001f ? -toTarget.normalized : -transform.forward;
            float best = toTarget.sqrMagnitude;
            Vector3 destination = transform.position;
            for (int i = -2; i <= 2; i++)
            {
                Vector3 point = transform.position + Quaternion.Euler(0, i * 40, 0) * away * 4;
                if (!NavMesh.SamplePosition(point, out var hit, 1.5f, NavMesh.AllAreas) ||
                    !_agent.CalculatePath(hit.position, _candidatePath) || _candidatePath.status != NavMeshPathStatus.PathComplete)
                {
                    continue;
                }
                
                float score = (hit.position - _target.transform.position).sqrMagnitude;
                if (score > best) 
                { 
                    best = score;
                    destination = hit.position;
                    
                }
            }

            if (destination == transform.position)
            {
                Stop();
                return;
            }
            
            _agent.isStopped = false;
            _agent.stoppingDistance = 0.2f;
            _agent.SetDestination(destination);
        }
    }
}
