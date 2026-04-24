using System.Collections.Generic;
using Actors.Enemies.BehaviourTree.Base;
using UnityEngine;
using UnityEngine.AI;

namespace Actors.Enemies.BehaviourTree
{
    public abstract class EnemyBehaviourTree : BehaviourTree.Base.BehaviourTree
    {
        [Header("Enemy Settings")]
        [SerializeField] protected float attackDamage = 10f;
        [SerializeField] protected float attackRange = 2f;
        [SerializeField] protected float attackCooldown = 1.5f;
        [SerializeField] protected float detectRange = 10f;
        [SerializeField] protected string attackTriggerName = "Attack";

        protected NavMeshAgent Agent;
        protected Animator Animator;
        protected Transform PlayerTarget;

        protected override void Awake()
        {
            Agent = GetComponent<NavMeshAgent>();
            Animator = GetComponent<Animator>();
            FindPlayer();
            base.Awake(); // вызовет SetupTree()
        }

        private void FindPlayer()
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                PlayerTarget = playerObj.transform;
            else
                Debug.LogError("Player not found! Enemies won't work.");
        }

        protected override Node SetupTree()
        {
            // Корневой узел сохраняет цель в DataContext, чтобы все дочерние узлы имели доступ.
            Node root = new Selector(); // основной селектор приоритетов
            root.SetData("Target", PlayerTarget);
            root.SetChildren(BuildTree(root));
            return root;
        }

        /// <summary>
        /// Абстрактный метод, который должен вернуть список дочерних узлов для корневого селектора.
        /// </summary>
        protected abstract List<Node> BuildTree(Node rootContext);
    }
}