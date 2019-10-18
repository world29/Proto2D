using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D.AI
{
    public class IsPlayerInSight : Decorator
    {
        public override NodeStatus Evaluate(EnemyBehaviour enemyBehaviour)
        {
            if (enemyBehaviour.IsPlayerInSight() || m_nodeStatus == NodeStatus.RUNNING)
            {
                m_nodeStatus = m_node.Evaluate(enemyBehaviour);
            }
            else
            {
                m_nodeStatus = NodeStatus.FAILURE;
            }

            return m_nodeStatus;
        }
    }
}
