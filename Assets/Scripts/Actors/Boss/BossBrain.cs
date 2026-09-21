using Actors.Animations;
using Actors.Boss.States;
using Actors.Enemies;
using Actors.Stats;
using UnityEngine;
using UnityEngine.AI;

namespace Actors.Boss
{
    public sealed class BossBrain : MonoBehaviour
    {
        private Combatant _actor;
        private Combatant _target;
        private NavMeshAgent _agent;
        private ICharacterAnimation _animation;
        private BossContext _context;
        private StateMachine.StateMachine _stateMachine;

        public string CurrentState => _stateMachine.CurrentStateName;

        public void Initialize(Combatant owner, Combatant target,  ActorCombat combat,
            ICharacterAnimation anim, CharacterDefinition definition, EnemyMode mode)
        {
            _actor = owner;
            _target = target;
            _animation = anim;
            _agent = GetComponent<NavMeshAgent>();

            _context = new BossContext(transform, owner, target, _agent, combat, definition, mode);
            _stateMachine = new StateMachine.StateMachine();

            _stateMachine.Add(new BossIdleState(_context, _stateMachine));
            _stateMachine.Add(new BossAggressionState(_context, _stateMachine));
            _stateMachine.Add(new BossChaseState(_context, _stateMachine));
            _stateMachine.Add(new BossAttackState(_context, _stateMachine));
            _stateMachine.Add(new BossStrongAttackState(_context, _stateMachine));
            _stateMachine.Add(new BossRepositionState(_context, _stateMachine));
            _stateMachine.Add(new BossDeadState(_context, _stateMachine));

            _actor.Health.Damaged += OnDamaged;
            _actor.Health.Died += OnDied;

            _stateMachine.Change<BossIdleState>();
        }

        private void Update()
        {
            if (!_actor.IsAlive || !_target.IsAlive)
                return;

            if (!_agent.enabled || !_agent.isOnNavMesh)
                return;

            if (_context.Combat.IsLocked)
            {
                _context.Stop();
                _animation.SetSpeed(0f);
                return;
            }

            _stateMachine.Tick();
            _animation.SetSpeed(_agent.velocity.magnitude);
        }

        private void OnDamaged(Damage damage)
        {
            _context.WasAttacked = true;
        }

        private void OnDied()
        {
            _stateMachine.Change<BossDeadState>();
            _animation.SetSpeed(0f);
        }

        private void OnDestroy()
        {
            _actor.Health.Damaged -= OnDamaged;
            _actor.Health.Died -= OnDied;
        }
    }
}