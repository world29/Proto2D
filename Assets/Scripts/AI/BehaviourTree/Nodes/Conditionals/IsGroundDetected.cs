using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D.AI
{
    [CreateNodeMenu("BehaviourTree/Conditional/IsGroundDetected")]
    public class IsGroundDetected: Conditional
    {
        public override NodeStatus Evaluate(EnemyBehaviour enemyBehaviour)
        {
            m_nodeStatus = enemyBehaviour.IsGroundDetected()
                            ? NodeStatus.SUCCESS
                            : NodeStatus.FAILURE;

            return m_nodeStatus;
        }
    }
}
