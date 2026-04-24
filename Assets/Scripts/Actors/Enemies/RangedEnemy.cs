using System.Collections.Generic;
using Actors.Enemies.BehaviourTree;
using Actors.Enemies.BehaviourTree.Nodes;
using Actors.Enemies.BehaviourTree.Base;
using UnityEngine;

namespace Actors.Enemies
{
    public class RangedEnemy : EnemyBehaviourTree
    {
        [SerializeField] private float preferredMinDist = 5f;
        [SerializeField] private float preferredMaxDist = 10f;

        protected override List<Node> BuildTree(Node rootContext)
        {
            // 1. Атаковать, если игрок внутри комфортной дистанции (5-10 м)
            var attackInRange = new Sequence(new List<Node>
            {
                new CheckDistanceCondition(transform, preferredMinDist, preferredMaxDist),
                new AttackAction(Animator, transform, attackDamage, attackRange, attackCooldown, attackTriggerName)
            });

            // 2. Если игрок слишком далеко (>10 м), идти к нему, пока не войдёт в зону атаки
            var approachSequence = new Sequence(new List<Node>
            {
                new CheckDistanceCondition(transform, preferredMaxDist, Mathf.Infinity),
                new MoveToTargetAction(Agent, transform, preferredMaxDist - 1f, false)
            });

            // 3. Если игрок слишком близко (<5 м), отходить
            var retreatSequence = new Sequence(new List<Node>
            {
                new CheckDistanceCondition(transform, 0f, preferredMinDist),
                new MoveToTargetAction(Agent, transform, preferredMinDist + 1f, true)
            });

            return new List<Node>
            {
                attackInRange,
                approachSequence,
                retreatSequence,
                new IdleAction()
            };
        }
    }
}