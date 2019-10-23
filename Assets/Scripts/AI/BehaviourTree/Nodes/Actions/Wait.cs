using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D.AI
{
    [CreateNodeMenu("BehaviourTree/Action/Wait")]
    public class Wait : Action
    {
        public RandomValue m_timeout;

        private float m_timeToWait;
        private float m_timeWaitStarted;

        // override XNode.Init()
        protected override void Init()
        {
            base.Init();

            m_timeToWait = 0;
            m_timeWaitStarted = 0;
        }

        public override NodeStatus Evaluate(EnemyBehaviour enemyBehaviour)
        {
            if (m_nodeStatus == NodeStatus.READY)
            {
                // タイムアウト時間を設定
                m_timeToWait = m_timeout.Value;
                Debug.LogFormat("Wait timeout = {0}", m_timeToWait);

                m_timeWaitStarted = Time.timeSinceLevelLoad;

                m_nodeStatus = NodeStatus.RUNNING;
            }
            else if ((Time.timeSinceLevelLoad - m_timeWaitStarted) >= m_timeToWait)
            {
                m_nodeStatus = NodeStatus.SUCCESS;
            }

            return m_nodeStatus;
        }

        public override void Abort()
        {
            base.Abort();

            m_timeToWait = 0;
            m_timeWaitStarted = 0;
        }

        public override float GetProgress()
        {
            return (Time.timeSinceLevelLoad - m_timeWaitStarted) / m_timeToWait;
        }
    }
}
