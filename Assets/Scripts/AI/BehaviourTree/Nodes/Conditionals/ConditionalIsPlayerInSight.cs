using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D.AI
{
    [CreateNodeMenu("BehaviourTree/Conditional/IsPlayerInSight")]
    public class ConditionalIsPlayerInSight : Conditional
    {
        public int m_sightIndex = 0;

        public override NodeStatus Evaluate(EnemyBehaviour enemyBehaviour)
        {
            m_nodeStatus = enemyBehaviour.IsPlayerInSight(m_sightIndex)
                            ? NodeStatus.SUCCESS
                            : NodeStatus.FAILURE;

            return m_nodeStatus;
        }
    }
}
