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

        protected override void OnReady()
        {
            base.OnReady();

            // タイムアウト値を更新
            m_timeToWait = m_timeout.Value;
        }

        public override NodeStatus Evaluate(EnemyBehaviour enemyBehaviour)
        {
            if (m_nodeStatus == NodeStatus.READY)
            {
                m_timeWaitStarted = Time.timeSinceLevelLoad;

                m_nodeStatus = NodeStatus.RUNNING;
            }
            else if ((Time.timeSinceLevelLoad - m_timeWaitStarted) >= m_timeToWait)
            {
                m_nodeStatus = NodeStatus.SUCCESS;
            }

            return m_nodeStatus;
        }

        protected override void OnAbort()
        {
            m_timeToWait = 0;
            m_timeWaitStarted = 0;
        }

        public override float GetProgress()
        {
            return (Time.timeSinceLevelLoad - m_timeWaitStarted) / m_timeToWait;
        }
    }
}
