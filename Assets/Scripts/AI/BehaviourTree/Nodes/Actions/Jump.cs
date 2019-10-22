using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D.AI
{
    [CreateNodeMenu("BehaviourTree/Action/Jump")]
    public class Jump : Action
    {
        public Vector2 jumpVelocity = Vector2.one;

        public override NodeStatus Evaluate(EnemyBehaviour enemyBehaviour)
        {
            if (m_nodeStatus == NodeStatus.READY)
            {
                enemyBehaviour.Jump(jumpVelocity);

                m_nodeStatus = NodeStatus.RUNNING;
            }
            else if (enemyBehaviour.IsOnGround())
            {
                m_nodeStatus = NodeStatus.SUCCESS;
            }

            return m_nodeStatus;
        }

        public override float GetProgress()
        {
            return 1;
        }
    }
}
