using Actors.Animations;
using Actors.Enemies.States;
using Actors.Stats;
using Combat;
using StateMachine;
using UnityEngine;
using UnityEngine.AI;

namespace Actors.Enemies
{
    public sealed class EnemyBrain : MonoBehaviour
    {
        private Combatant _actor;
        private Combatant _target;
        private NavMeshAgent _agent;
        private ICharacterAnimation _animation;
        private EnemyContext _context;
        private StateMachine.StateMachine _stateMachine;

        public string CurrentState => _stateMachine.CurrentStateName;

        public void Initialize(Combatant owner, Combatant target, ActorCombat combat,
            ICharacterAnimation anim, CharacterDefinition definition, EnemyMode mode, bool ranged,
            AttackAnimation attackAnimation, bool requiresPreparation, float preparationDuration)
        {
            _actor = owner;
            _target = target;
            _agent = GetComponent<NavMeshAgent>();
            _animation = anim;

            _context = new EnemyContext(transform, owner, target, _agent, combat, anim, definition, mode, ranged,
                attackAnimation, requiresPreparation, preparationDuration);

            _stateMachine = new StateMachine.StateMachine();

            _stateMachine.Add(new EnemyIdleState(_context, _stateMachine));
            _stateMachine.Add(new EnemyAggressionState(_context, _stateMachine));
            _stateMachine.Add(new EnemyPrepareAttackState(_context, _stateMachine));
            _stateMachine.Add(new EnemyAttackState(_context, _stateMachine));
            _stateMachine.Add(new EnemyFleeState(_context, _stateMachine));
            _stateMachine.Add(new EnemyDeadState(_context, _stateMachine));

            _actor.Health.Damaged += OnDamaged;
            _actor.Health.Died += OnDied;

            _stateMachine.Change<EnemyIdleState>();
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
            _stateMachine.Change<EnemyDeadState>();
            _animation.SetSpeed(0f);
        }

        private void OnDestroy()
        {
            _actor.Health.Damaged -= OnDamaged;
            _actor.Health.Died -= OnDied;
        }
    }
}