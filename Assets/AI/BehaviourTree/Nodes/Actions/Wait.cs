using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D.AI
{
    public class Wait : Action
    {
        public float timeout = 1;

        private float timeWaitStart;

        public override NodeStatus Evaluate(EnemyBehaviour context)
        {
            if (m_nodeStatus == NodeStatus.READY)
            {
                timeWaitStart = Time.timeSinceLevelLoad;

                m_nodeStatus = NodeStatus.RUNNING;
            }
            else if ((Time.timeSinceLevelLoad - timeWaitStart) >= timeout)
            {
                m_nodeStatus = NodeStatus.SUCCESS;
            }

            return m_nodeStatus;
        }

        public override float GetProgress()
        {
            return (Time.timeSinceLevelLoad - timeWaitStart) / timeout;
        }
    }
}
