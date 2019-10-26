using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D.AI
{
    [CreateNodeMenu("BehaviourTree/Decorator/IsPlayerInSight")]
    public class IsPlayerInSight : Decorator
    {
        public int m_sightIndex = 0;

        public override NodeStatus Evaluate(EnemyBehaviour enemyBehaviour)
        {
            if (enemyBehaviour.IsPlayerInSight(m_sightIndex) || m_nodeStatus == NodeStatus.RUNNING)
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
