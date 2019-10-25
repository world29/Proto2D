using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D.AI
{
    [CreateNodeMenu("BehaviourTree/Selector")]
    public class Selector : Composite
    {
        public override NodeStatus Evaluate(EnemyBehaviour enemyBehaviour)
        {
            for (int i = m_nodeIndex; i < m_nodes.Count; i++)
            {
                m_nodeStatus = m_nodes[i].Evaluate(enemyBehaviour);

                if (m_nodeStatus == NodeStatus.FAILURE)
                {
                    // 次回の評価は先頭ノードから開始する
                    m_nodeIndex = 0;

                    // 次のノードを評価する
                    continue;
                }
                else if (m_nodeStatus == NodeStatus.RUNNING)
                {
                    // 次回の評価はこのノードから開始する
                    m_nodeIndex = i;

                    // 次のノードを評価しない
                    break;
                }
                else if (m_nodeStatus == NodeStatus.SUCCESS)
                {
                    // 次回の評価は先頭ノードから開始する
                    m_nodeIndex = 0;

                    // 次のノードを評価しない
                    break;
                }
                else
                {
                    Debug.AssertFormat(false, "invalid node status: {0}", m_nodeStatus);
                    break;
                }
            }

            return m_nodeStatus;
        }

        public override void Abort()
        {
            base.Abort();

            m_nodeIndex = 0;
        }
    }
}
