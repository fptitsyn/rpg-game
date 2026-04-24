using Actors.Enemies.BehaviourTree.Base;
using UnityEngine;

namespace Actors.Enemies.BehaviourTree.Nodes
{
    /// <summary>
    /// Проверяет, находится ли цель в заданном диапазоне расстояний.
    /// Параметры: MinDistance, MaxDistance.
    /// Цель берётся из Data контекста по ключу "Target".
    /// </summary>
    public class CheckDistanceCondition : ConditionNode
    {
        private Transform _self;
        private float _minDist;
        private float _maxDist;
        private string _targetKey;

        public CheckDistanceCondition(Transform self, float minDist, float maxDist, string targetKey = "Target")
        {
            _self = self;
            _minDist = minDist;
            _maxDist = maxDist;
            _targetKey = targetKey;
        }

        protected override bool CheckCondition()
        {
            object targetObj = GetData(_targetKey);
            if (targetObj == null) return false;
            Transform target = (Transform)targetObj;
            float dist = Vector3.Distance(_self.position, target.position);
            return dist >= _minDist && dist <= _maxDist;
        }
    }
}