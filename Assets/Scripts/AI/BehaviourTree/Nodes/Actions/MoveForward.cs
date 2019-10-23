using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D.AI
{
    [CreateNodeMenu("BehaviourTree/Action/MoveForward")]
    public class MoveForward : Action
    {
        public float m_speed = 1;

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
            enemyBehaviour.MoveForward(m_speed);

            if (m_nodeStatus == NodeStatus.READY)
            {
                m_timeToWait = m_timeout.Value;
                Debug.LogFormat("MoveForward timeout = {0}", m_timeToWait);

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
