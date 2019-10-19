using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D.AI
{
    [CreateNodeMenu("BehaviourTree/Action/Wait")]
    public class Wait : Action
    {
        //public Vector2 timeoutMinMax = new Vector2(1f,1.5f);
        // うまくいかなかったのでtimeoutをpublicのまま直接指定
        public float timeout = 1;

        private float m_timeWaitStarted;

        // override XNode.Init()
        protected override void Init()
        {
            base.Init();

            m_timeWaitStarted = 0;
        }

        public override NodeStatus Evaluate(EnemyBehaviour enemyBehaviour)
        {
            if (m_nodeStatus == NodeStatus.READY)
            {
                m_timeWaitStarted = Time.timeSinceLevelLoad;

                m_nodeStatus = NodeStatus.RUNNING;
            }
            else if ((Time.timeSinceLevelLoad - m_timeWaitStarted) >= timeout)
            {
                m_nodeStatus = NodeStatus.SUCCESS;
            }

            return m_nodeStatus;
        }

        public override void Abort()
        {
            base.Abort();

            m_timeWaitStarted = 0;
        }

        public override float GetProgress()
        {
            return (Time.timeSinceLevelLoad - m_timeWaitStarted) / timeout;
        }
    }
}
