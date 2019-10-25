using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Proto2D.AI
{
    [CreateNodeMenu("BehaviourTree/Sequence")]
    public class Sequence : Composite
    {
        public override void Setup()
        {
            base.Setup();

            SetupNodesOrderByPriority();
        }

        protected override void OnReady()
        {
            m_nodeIndex = 0;
        }

        public override NodeStatus Evaluate(EnemyBehaviour enemyBehaviour)
        {
            int i = m_nodeIndex;
            for (; i < m_nodes.Count; i++)
            {
                m_nodeStatus = m_nodes[i].Evaluate(enemyBehaviour);

                if (m_nodeStatus == NodeStatus.FAILURE)
                {
                    // 次回の評価は先頭ノードから開始する
                    //m_nodeIndex = 0;

                    // 次のノードを評価しない
                    break;
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
                    // 末尾ノードが SUCCESS なら、次回の評価は先頭ノードから開始する
                    /*
                    if (i == m_nodes.Count - 1)
                    {
                        m_nodeIndex = 0;
                    }
                    */

                    // 次のノードを評価する
                    continue;
                }
                else
                {
                    Debug.AssertFormat(false, "invalid node status: {0}", m_nodeStatus);
                    break;
                }
            }

            return m_nodeStatus;
        }

        public override void OnCreateConnection(XNode.NodePort from, XNode.NodePort to)
        {
            base.OnCreateConnection(from, to);

            // children detached
            if (Outputs.Contains(from) || Outputs.Contains(to))
            {
                SetupNodesOrderByPriority();
            }
        }

        public override void OnRemoveConnection(XNode.NodePort port)
        {
            base.OnRemoveConnection(port);

            // children detached
            if (Outputs.Contains(port))
            {
                SetupNodesOrderByPriority();
            }
        }
    }
}
