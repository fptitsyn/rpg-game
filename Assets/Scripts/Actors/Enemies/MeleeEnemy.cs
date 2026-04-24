using System.Collections.Generic;
using Actors.Enemies.BehaviourTree;
using Actors.Enemies.BehaviourTree.Nodes;
using Actors.Enemies.BehaviourTree.Base;

namespace Actors.Enemies
{
    public class MeleeEnemy : EnemyBehaviourTree
    {
        protected override List<Node> BuildTree(Node rootContext)
        {
            // Приоритеты:
            // 1. Атаковать, если игрок в радиусе атаки
            // 2. Преследовать, если игрок в радиусе обнаружения
            // 3. Ждать (Idle)

            var attackSequence = new Sequence(new List<Node>
            {
                new CheckDistanceCondition(transform, 0f, attackRange),
                new AttackAction(Animator, transform, attackDamage, attackRange, attackCooldown, attackTriggerName)
            });

            var chaseSequence = new Sequence(new List<Node>
            {
                new CheckDistanceCondition(transform, 0f, detectRange),
                new MoveToTargetAction(Agent, transform, attackRange * 0.9f, false) // подойти чуть ближе минимальной дистанции
            });

            return new List<Node>
            {
                attackSequence,
                chaseSequence,
                new IdleAction()
            };
        }
    }
}