using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D.AI
{
    [CreateNodeMenu("BehaviourTree/Conditional/IsOnGround")]
    public class ConditionalIsOnGround : Conditional
    {
        public override NodeStatus Evaluate(EnemyBehaviour enemyBehaviour)
        {
            m_nodeStatus = enemyBehaviour.IsOnGround()
                            ? NodeStatus.SUCCESS
                            : NodeStatus.FAILURE;

            return m_nodeStatus;
        }
    }
}
