using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D.AI
{
    public class MoveForward : Action
    {
        public float speed = 1;
        public float timeout = 1;

        private float m_timeWaitStart = 0;

        public override NodeStatus Evaluate(EnemyBehaviour enemyBehaviour)
        {
            enemyBehaviour.MoveForward(speed);

            if (m_nodeStatus == NodeStatus.READY)
            {
                m_timeWaitStart = Time.timeSinceLevelLoad;

                m_nodeStatus = NodeStatus.RUNNING;
            }
            else if ((Time.timeSinceLevelLoad - m_timeWaitStart) >= timeout)
            {
                m_nodeStatus = NodeStatus.SUCCESS;
            }

            return m_nodeStatus;
        }

        public override void Abort()
        {
            base.Abort();

            m_timeWaitStart = 0;
        }

        public override float GetProgress()
        {
            return (Time.timeSinceLevelLoad - m_timeWaitStart) / timeout;
        }
    }
}
