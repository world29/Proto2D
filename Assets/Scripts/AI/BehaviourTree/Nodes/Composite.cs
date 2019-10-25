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
            m_nodeIndex = 0;
        }

        public override void Setup()
        {
            base.Setup();

            RefreshNodes();
        }

        [ContextMenu("Refresh Nodes")]
        public void RefreshNodes()
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

        public override void OnCreateConnection(XNode.NodePort from, XNode.NodePort to)
        {
            // children detached
            if (Outputs.Contains(from) || Outputs.Contains(to))
            {
                RefreshNodes();
            }
        }

        public override void OnRemoveConnection(XNode.NodePort port)
        {
            base.OnRemoveConnection(port);

            // children detached
            if (Outputs.Contains(port))
            {
                RefreshNodes();
            }
        }
    }
}
