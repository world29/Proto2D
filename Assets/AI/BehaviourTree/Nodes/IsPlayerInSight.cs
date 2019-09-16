using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D.AI
{
    public class IsPlayerInSight : Decorator
    {
        public override NodeStatus Evaluate(EnemyBehaviour context)
        {
            if (context.IsPlayerInSight() || m_nodeStatus == NodeStatus.RUNNING)
            {
                m_nodeStatus = m_node.Evaluate(context);
            }
            else
            {
                m_nodeStatus = NodeStatus.FAILURE;
            }

            return m_nodeStatus;
        }

    }
}
