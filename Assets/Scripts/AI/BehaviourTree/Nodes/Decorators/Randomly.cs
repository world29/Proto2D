using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D.AI
{
    [CreateNodeMenu("BehaviourTree/Decorator/Randomly")]
    public class Randomly : Decorator
    {
        [Range(0, 100), Tooltip("子ノードが評価される確率を百分率で指定")]
        public int m_percentage = 50;

        private int m_randomValue = -1;

        protected override void OnReady()
        {
            base.OnReady();

            m_randomValue = Random.Range(0, 100);
        }

        public override NodeStatus Evaluate(EnemyBehaviour enemyBehaviour)
        {
            if (m_nodeStatus == NodeStatus.RUNNING || m_randomValue < m_percentage)
            {
                m_nodeStatus = m_node.Evaluate(enemyBehaviour);
            }
            else
            {
                m_nodeStatus = NodeStatus.FAILURE;
            }

            return m_nodeStatus;
        }
    }
}
