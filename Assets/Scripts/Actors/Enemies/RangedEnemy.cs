using UnityEngine;

namespace Actors.Enemies
{
    public class RangedEnemy : EnemyStateMachine
    {
        [Header("Ranged Settings")]
        [SerializeField] private float minAttackDistance = 5f;
        [SerializeField] private float maxAttackDistance = 10f;

        protected override void EnterState(EnemyState state)
        {
            switch (state)
            {
                case EnemyState.Idle:
                    agent.isStopped = true;
                    break;
                case EnemyState.Flee:
                case EnemyState.Chase:
                    agent.isStopped = false;
                    break;
                case EnemyState.Attack:
                    agent.isStopped = true;
                    attackInProgress = false;
                    attackTimer = 0f;
                    break;
                case EnemyState.Dead:
                    agent.isStopped = true;
                    animator.SetTrigger("Death");
                    enabled = false;
                    break;
            }
        }

        protected override void UpdateState(EnemyState state)
        {
            if (!player) return;
            float dist = DistanceToPlayer();
            
            switch (state)
            {
                case EnemyState.Idle:
                    LookAtPlayer();
                    if (dist <= detectionRadius)
                    {
                        if (actor.CurrentHealth < fleeHealthThreshold)
                            ChangeState(EnemyState.Flee);
                        else if (dist >= minAttackDistance && dist <= maxAttackDistance)
                            ChangeState(EnemyState.Attack);
                        else if (dist > maxAttackDistance)
                            ChangeState(EnemyState.Chase);
                        else if (dist < minAttackDistance)
                            ChangeState(EnemyState.Flee);
                    }
                    break;

                case EnemyState.Chase:
                    if (actor.CurrentHealth < fleeHealthThreshold)
                    {
                        ChangeState(EnemyState.Flee);
                        return;
                    }
                    if (dist >= minAttackDistance && dist <= maxAttackDistance)
                    {
                        ChangeState(EnemyState.Attack);
                        return;
                    }
                    if (dist > maxAttackDistance)
                    {
                        agent.SetDestination(player.position);
                        LookAtPlayer();
                    }
                    else if (dist < minAttackDistance)
                    {
                        ChangeState(EnemyState.Flee);
                    }
                    if (dist > detectionRadius)
                        ChangeState(EnemyState.Idle);
                    break;

                case EnemyState.Attack:
                    if (actor.CurrentHealth < fleeHealthThreshold)
                    {
                        ChangeState(EnemyState.Flee);
                        return;
                    }
                    if (dist < minAttackDistance || dist > maxAttackDistance)
                    {
                        ChangeState(dist < minAttackDistance ? EnemyState.Flee : EnemyState.Chase);
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
                            var playerActor = player.GetComponent<Actor>();
                            playerActor?.ReceiveDamage(attackDamage);
                            damageDealt = true;
                        }
                        if (Time.time >= attackTimer)
                        {
                            attackInProgress = false;
                            float d = DistanceToPlayer();
                            if (d >= minAttackDistance && d <= maxAttackDistance)
                                ChangeState(EnemyState.Attack);
                            else
                                ChangeState(d < minAttackDistance ? EnemyState.Flee : EnemyState.Chase);
                        }
                    }
                    break;

                case EnemyState.Flee:
                    if (actor.CurrentHealth > fleeHealthThreshold && dist >= minAttackDistance)
                    {
                        ChangeState(EnemyState.Idle);
                        return;
                    }
                    Vector3 dirAway = (transform.position - player.position).normalized;
                    agent.SetDestination(transform.position + dirAway * fleeDistance);
                    if (dist > fleeDistance)
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