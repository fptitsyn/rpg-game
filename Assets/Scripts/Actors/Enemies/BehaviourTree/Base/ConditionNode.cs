namespace Actors.Enemies.BehaviourTree.Base
{
    /// <summary>
    /// Базовый класс для условий. Не изменяет состояние мира, только проверяет.
    /// Возвращает Success или Failure (никогда Running).
    /// </summary>
    public abstract class ConditionNode : Node
    {
        public override NodeState Evaluate() => CheckCondition() ? NodeState.Success : NodeState.Failure;
        protected abstract bool CheckCondition();
    }
}