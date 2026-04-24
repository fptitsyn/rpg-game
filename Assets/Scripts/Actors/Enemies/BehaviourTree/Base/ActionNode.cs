namespace Actors.Enemies.BehaviourTree.Base
{
    /// <summary>
    /// Базовый класс для листовых узлов-действий.
    /// Действие может вернуть Running, Success или Failure.
    /// </summary>
    public abstract class ActionNode : Node
    {
        // Обычно действия не имеют детей
        public override NodeState Evaluate() => PerformAction();

        protected abstract NodeState PerformAction();
    }
}