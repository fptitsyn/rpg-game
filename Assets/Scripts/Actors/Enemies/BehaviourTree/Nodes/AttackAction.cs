using Actors.Enemies.BehaviourTree.Base;
using UnityEngine;

namespace Actors.Enemies.BehaviourTree.Nodes
{
    /// <summary>
    /// Узел атаки. Проигрывает анимацию, наносит урон после задержки.
    /// Использует кулдаун, чтобы не атаковать каждый кадр.
    /// Возвращает Running во время подготовки/атаки, Success после нанесения урона.
    /// </summary>
    public class AttackAction : ActionNode
    {
        private Animator _animator;
        private Transform _self;
        private float _damage;
        private float _attackRange;
        private float _cooldown;
        private float _lastAttackTime = -Mathf.Infinity;
        private bool _damageDealt = false;
        private float _attackDuration = 0.6f; // длительность анимации
        private string _targetKey;
        private string _triggerName;
        private int _triggerHash;

        public AttackAction(Animator animator, Transform self, float damage, float attackRange, 
                            float cooldown, string triggerName, string targetKey = "Target")
        {
            _animator = animator;
            _self = self;
            _damage = damage;
            _attackRange = attackRange;
            _cooldown = cooldown;
            _triggerName = triggerName;
            _triggerHash = Animator.StringToHash(triggerName);
            _targetKey = targetKey;
        }

        protected override NodeState PerformAction()
        {
            // Проверка кулдауна
            if (Time.time - _lastAttackTime < _cooldown)
                return NodeState.Failure; // ещё не готов

            object targetObj = GetData(_targetKey);
            if (targetObj == null) return NodeState.Failure;
            Transform target = (Transform)targetObj;

            float dist = Vector3.Distance(_self.position, target.position);
            if (dist > _attackRange) return NodeState.Failure;

            // Старт атаки
            if (!_damageDealt)
            {
                _animator.SetTrigger(_triggerHash);
                _damageDealt = false;
                _lastAttackTime = Time.time; // начало атаки
            }

            // Ожидаем завершения анимации (упрощённо через таймер)
            // Можно было бы использовать события анимации или StateMachineBehaviour,
            // но для простоты используем время.
            if (Time.time - _lastAttackTime >= _attackDuration)
            {
                // Нанести урон
                var playerHealth = target.GetComponent<Player.Player>();
                if (playerHealth && dist <= _attackRange)
                    playerHealth.ReceiveDamage(_damage);
                _damageDealt = false;
                return NodeState.Success;
            }

            return NodeState.Running;
        }
    }
}