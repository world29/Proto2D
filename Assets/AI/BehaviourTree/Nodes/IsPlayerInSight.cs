using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D.AI
{
    public class IsPlayerInSight : Decorator
    {

#if UNITY_EDITOR
        private NodeStatus m_nodeStatus = NodeStatus.SUCCESS;
#endif

        public override void PrepareForEvaluation(BehaviourTreeContext context)
        {
            NodeContext nodeContext = context.dict.Get<NodeContext>(GetInstanceID());
            if (nodeContext.nodeStatus != NodeStatus.RUNNING)
            {
                nodeContext.nodeStatus = NodeStatus.READY;
            }
        }

        public override NodeStatus Evaluate(BehaviourTreeContext context)
        {
            NodeContext nodeContext = context.dict.Get<NodeContext>(GetInstanceID());

            if (context.enemy.IsPlayerInSight() || nodeContext.nodeStatus == NodeStatus.RUNNING)
            {
                nodeContext.nodeStatus = m_node.Evaluate(context);
            }
            else
            {
                nodeContext.nodeStatus = NodeStatus.FAILURE;
            }

            context.dict.Store(GetInstanceID(), nodeContext);

#if UNITY_EDITOR
            m_nodeStatus = nodeContext.nodeStatus;
#endif

            return nodeContext.nodeStatus;
        }

        public override void Abort(BehaviourTreeContext context)
        {
            context.dict.Remove(GetInstanceID());
        }

#if UNITY_EDITOR
        public override NodeStatus GetStatus()
        {
            return m_nodeStatus;
        }
#endif

    }
}
