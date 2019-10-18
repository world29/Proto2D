using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D.AI
{
    public class Selector : Node
    {
        [Output] public List<Node> children;

        private List<Node> m_nodes = new List<Node>();

        private int m_nodeIndex;

        protected override void Init()
        {
            //
            m_nodes.Clear();

            XNode.NodePort outPort = GetOutputPort("children");
            if (!outPort.IsConnected)
            {
                return;
            }

            List<XNode.NodePort> connections = outPort.GetConnections();
            foreach (XNode.NodePort port in connections)
            {
                m_nodes.Add(port.node as Node);
            }
        }

        public override NodeStatus Evaluate(EnemyBehaviour enemyBehaviour)
        {
            for (int i = m_nodeIndex; i < m_nodes.Count; i++)
            {
                m_nodeStatus = m_nodes[i].Evaluate(enemyBehaviour);
                switch (m_nodeStatus)
                {
                    case NodeStatus.FAILURE:
                    case NodeStatus.SUCCESS:
                        // 次回の評価は先頭ノードから開始する
                        m_nodeIndex = 0;
                        break;
                    case NodeStatus.RUNNING:
                        // 次回の評価はこのノードから開始する
                        m_nodeIndex = i;
                        break;
                    default:
                        Debug.Assert(false, "Invalid Node Status");
                        break;
                }

                // FAILURE の場合のみ、次のノードを評価する
                if (m_nodeStatus != NodeStatus.FAILURE)
                {
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
