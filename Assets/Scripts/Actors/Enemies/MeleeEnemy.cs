using UnityEngine;

namespace Actors.Enemies
{
    public class MeleeEnemy : EnemyStateMachine
    {
        [SerializeField] private float attackRange = 2f;
        
        protected override void EnterState(EnemyState state)
        {
            switch (state)
            {
                case EnemyState.Idle:
                    agent.isStopped = true;
                    break;
                case EnemyState.Chase:
                    agent.isStopped = false;
                    break;
                case EnemyState.Attack:
                    agent.isStopped = true;
                    attackInProgress = false;
                    attackTimer = 0f;
                    break;
                case EnemyState.Flee:
                    agent.isStopped = false;
                    break;
                case EnemyState.Dead:
                    agent.isStopped = true;
                    animator.SetTrigger("Death");
                    enabled = false; // отключаем обновления
                    break;
            }
        }

        protected override void UpdateState(EnemyState state)
        {
            if (player == null) return;
            float dist = DistanceToPlayer();

            switch (state)
            {
                case EnemyState.Idle:
                    LookAtPlayer();
                    if (dist <= detectionRadius)
                    {
                        if (actor.CurrentHealth < fleeHealthThreshold)
                            ChangeState(EnemyState.Flee);
                        else
                            ChangeState(EnemyState.Chase);
                    }
                    break;

                case EnemyState.Chase:
                    if (actor.CurrentHealth < fleeHealthThreshold)
                    {
                        ChangeState(EnemyState.Flee);
                        return;
                    }
                    if (dist <= attackRange)
                    {
                        ChangeState(EnemyState.Attack);
                        return;
                    }
                    if (dist > detectionRadius)
                    {
                        ChangeState(EnemyState.Idle);
                        return;
                    }
                    agent.SetDestination(player.position);
                    LookAtPlayer();
                    break;

                case EnemyState.Attack:
                    if (actor.CurrentHealth < fleeHealthThreshold)
                    {
                        ChangeState(EnemyState.Flee);
                        return;
                    }
                    if (dist > attackRange)
                    {
                        ChangeState(EnemyState.Chase);
                        return;
                    }
                    LookAtPlayer();
                    if (!attackInProgress)
                    {
                        if (Time.time >= attackTimer)
                        {
                            animator.SetTrigger("Attack");
                            attackInProgress = true;
                            damageDealt = false;
                            attackTimer = Time.time + attackCooldown + attackAnimationDuration;
                        }
                    }
                    else
                    {
                        if (!damageDealt && Time.time >= attackTimer - attackAnimationDuration + damageDelay)
                        {
                            if (DistanceToPlayer() <= attackRange)
                            {
                                var playerActor = player.GetComponent<Actor>();
                                playerActor?.ReceiveDamage(attackDamage);
                            }
                            damageDealt = true;
                        }
                        if (Time.time >= attackTimer)
                        {
                            attackInProgress = false;
                            ChangeState(DistanceToPlayer() <= detectionRadius ? EnemyState.Chase : EnemyState.Idle);
                        }
                    }
                    break;

                case EnemyState.Flee:
                    if (actor.CurrentHealth > fleeHealthThreshold && DistanceToPlayer() > detectionRadius)
                    {
                        ChangeState(EnemyState.Idle);
                        return;
                    }
                    Vector3 dirAway = (transform.position - player.position).normalized;
                    agent.SetDestination(transform.position + dirAway * fleeDistance);
                    if (DistanceToPlayer() > fleeDistance)
                        ChangeState(EnemyState.Idle);
                    break;
            }
        }

        protected override void ExitState(EnemyState state)
        {
            switch (state)
            {
                case EnemyState.Chase:
                case EnemyState.Flee:
                    agent.ResetPath();
                    break;
                case EnemyState.Attack:
                    attackInProgress = false;
                    break;
            }
        }
    }
}