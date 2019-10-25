using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Proto2D.AI
{
    [NodeTint(0.7f, 0.7f, 1f)]
    public abstract class Composite : Node
    {
        [Output(ShowBackingValue.Never, ConnectionType.Multiple)] public Node children;

        protected List<Node> m_nodes;
        protected int m_nodeIndex;

        // override XNode.Init()
        protected override void Init()
        {
            base.Init();

            m_nodes = new List<Node>();
            m_nodeIndex = -1;
        }

        protected void SetupNodesOrderByPriority()
        {
            m_nodes.Clear();

            XNode.NodePort outPort = GetOutputPort("children");
            if (!outPort.IsConnected)
            {
                return;
            }

            m_nodes = outPort.GetConnections().Select(port => port.node as Node).ToList();

            // Node.Priority の値で降順にソート
            m_nodes.Sort((lhs, rhs) => rhs.Priority - lhs.Priority);

            // 子ノードの EvaluationOrder プロパティを更新
            foreach (Node node in m_nodes)
            {
                node.EvaluationOrder = m_nodes.IndexOf(node) + 1;
            }
        }

        public override void Abort()
        {
            base.Abort();

            m_nodeIndex = -1;
        }
    }
}
