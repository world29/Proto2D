using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D.AI
{
    [CreateNodeMenu("BehaviourTree/Action/MoveForward")]
    public class MoveForward : Action
    {
        public string m_stateName;
        public float m_speed = 1;
        public bool m_autoTurn = true;
        public bool m_autoGroundDetection = true;

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

            enemyBehaviour.MoveForward(m_speed, m_autoTurn, m_autoGroundDetection);

            if (m_nodeStatus == NodeStatus.READY)
            {
                m_timeWaitStarted = Time.timeSinceLevelLoad;
                Animator animator = enemyBehaviour.gameObject.GetComponent<Animator>();
                if (m_stateName != "" && animator && animator.isActiveAndEnabled)
                {
                    animator.Play(m_stateName);
                }

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
