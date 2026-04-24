using Actors.Enemies.BehaviourTree.Base;

namespace Actors.Enemies.BehaviourTree.Nodes
{
    public class IdleAction : ActionNode
    {
        protected override NodeState PerformAction() => NodeState.Success;
    }
}