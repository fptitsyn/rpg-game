using Actors.Enemies.BehaviourTree.Base;
using UnityEngine;
using UnityEngine.AI;

namespace Actors.Enemies.BehaviourTree.Nodes
{
    /// <summary>
    /// Задаёт NavMeshAgent следование к цели (вперёд или от цели).
    /// Возвращает Running пока не достигнет нужной дистанции, затем Success.
    /// </summary>
    public class MoveToTargetAction : ActionNode
    {
        private NavMeshAgent _agent;
        private Transform _self;
        private float _stoppingDistance;
        private bool _moveAway; // true — от цели, false — к цели
        private string _targetKey;

        public MoveToTargetAction(NavMeshAgent agent, Transform self, float stoppingDistance, bool moveAway = false, string targetKey = "Target")
        {
            _agent = agent;
            _self = self;
            _stoppingDistance = stoppingDistance;
            _moveAway = moveAway;
            _targetKey = targetKey;
        }

        protected override NodeState PerformAction()
        {
            object targetObj = GetData(_targetKey);
            if (targetObj == null) return NodeState.Failure;
            Transform target = (Transform)targetObj;

            if (_moveAway)
            {
                // Направление от цели
                Vector3 dirFromTarget = (_self.position - target.position).normalized;
                Vector3 destination = _self.position + dirFromTarget * _stoppingDistance;
                _agent.SetDestination(destination);
            }
            else
            {
                _agent.SetDestination(target.position);
            }

            // Проверяем, достигли ли мы нужной дистанции
            float dist = Vector3.Distance(_self.position, target.position);
            if (_moveAway)
            {
                if (dist >= _stoppingDistance) return NodeState.Success;
                return NodeState.Running;
            }
            else
            {
                if (dist <= _stoppingDistance) return NodeState.Success;
                return NodeState.Running;
            }
        }
    }
}