using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D.AI
{
    public class Sequence : Node
    {
        [Output] public List<Node> children;

        private List<Node> m_nodes = new List<Node>();
        private int m_nodeIndex;

        protected override void Init()
        {
            base.Init();

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

            //
            m_nodeIndex = 0;
        }

        public override NodeStatus Evaluate(EnemyBehaviour context)
        {
            int i = m_nodeIndex;
            for (; i < m_nodes.Count; i++)
            {
                m_nodeStatus = m_nodes[i].Evaluate(context);
                switch (m_nodeStatus)
                {
                    case NodeStatus.FAILURE:
                        // 次回の評価は先頭ノードから開始する
                        m_nodeIndex = 0;
                        break;
                    case NodeStatus.RUNNING:
                        // 次回の評価はこのノードから開始する
                        m_nodeIndex = i;
                        break;
                    case NodeStatus.SUCCESS:
                        break;
                    default:
                        Debug.Assert(false, "Invalid Node Status");
                        break;
                }

                // SUCCESS の場合のみ、次のノードを評価する
                if (m_nodeStatus != NodeStatus.SUCCESS)
                {
                    break;
                }
            }

            return m_nodeStatus;
        }
    }
}
