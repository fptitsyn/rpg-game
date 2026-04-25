using UnityEngine;
using UnityEngine.AI;

namespace Actors.Enemies
{
    public enum EnemyState
    {
        Idle,   // Покой
        Chase,  // Агрессия/преследование
        Attack, // Атака
        Flee,   // Бегство
        Dead    // Смерть
    }

    [RequireComponent(typeof(Actor), typeof(NavMeshAgent), typeof(Animator))]
    public abstract class EnemyStateMachine : MonoBehaviour
    {
        protected EnemyState currentState;
        protected float stateTimer;
        protected NavMeshAgent agent;
        protected Animator animator;
        protected Actor actor;

        [Header("Основные параметры")]
        [SerializeField] protected float detectionRadius = 25f;
        [SerializeField] protected float attackCooldown = 1.5f;
        [SerializeField] protected float attackDamage = 10f;
        [SerializeField] protected float attackAnimationDuration = 0.8f;
        [SerializeField] protected float damageDelay = 0.4f;   // Задержка до нанесения урона от начала анимации
        [SerializeField] protected float moveSpeed = 3.5f;
        [SerializeField] protected float fleeHealthThreshold = 30f;
        [SerializeField] protected float fleeDistance = 15f;   // Как далеко убегает

        protected Transform player;
        protected bool isStunned = false;
        protected float stunTimer = 0f;
        protected bool attackInProgress = false;
        protected float attackTimer = 0f;
        protected bool damageDealt = false;

        protected virtual void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            actor = GetComponent<Actor>();
            agent.speed = moveSpeed;

            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj) player = playerObj.transform;
            else Debug.LogError("EnemyStateMachine: Player not found!");
        }

        protected virtual void Start()
        {
            ChangeState(EnemyState.Idle);
        }

        protected virtual void Update()
        {
            if (isStunned)
            {
                stunTimer -= Time.deltaTime;
                if (stunTimer <= 0) isStunned = false;
                else return; // Стан замораживает всё поведение
            }

            animator.SetBool("Walking", agent.velocity.sqrMagnitude > 0.1f);
            UpdateState(currentState);
        }

        public void ChangeState(EnemyState newState)
        {
            if (newState == currentState) return;
            ExitState(currentState);
            currentState = newState;
            EnterState(currentState);
        }

        // Вызывается из Enemy.ReceiveDamage
        public virtual void OnDamageReceived(float stunDuration = 0.3f)
        {
            if (currentState == EnemyState.Dead) return;
            animator.SetTrigger("TakeDamage");
            isStunned = true;
            stunTimer = stunDuration;
            attackInProgress = false;

            if (actor.CurrentHealth <= 0)
                ChangeState(EnemyState.Dead);
        }

        protected abstract void EnterState(EnemyState state);
        protected abstract void UpdateState(EnemyState state);
        protected abstract void ExitState(EnemyState state);

        protected float DistanceToPlayer() =>
            player ? Vector3.Distance(transform.position, player.position) : Mathf.Infinity;

        protected void LookAtPlayer()
        {
            if (!player) return;
            var dir = (player.position - transform.position).normalized;
            dir.y = 0;
            if (dir != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10f * Time.deltaTime);
        }
    }
}