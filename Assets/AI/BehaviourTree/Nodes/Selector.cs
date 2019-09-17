using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D.AI
{
    public class Selector : Node
    {
        [Output] public List<Node> children;

        private List<Node> m_nodes = new List<Node>();

#if UNITY_EDITOR
        private NodeStatus m_nodeStatus = NodeStatus.SUCCESS;
#endif

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

        public override void PrepareForEvaluation(BehaviourTreeContext context)
        {
            SelectorNodeContext nodeContext = context.dict.Get<SelectorNodeContext>(GetInstanceID());
            if (nodeContext.nodeStatus != NodeStatus.RUNNING)
            {
                nodeContext.nodeStatus = NodeStatus.READY;
            }
        }

        public override NodeStatus Evaluate(BehaviourTreeContext context)
        {
            SelectorNodeContext nodeContext = context.dict.Get<SelectorNodeContext>(GetInstanceID());

            for (int i = nodeContext.nodeIndex; i < m_nodes.Count; i++)
            {
                nodeContext.nodeStatus = m_nodes[i].Evaluate(context);
                switch (nodeContext.nodeStatus)
                {
                    case NodeStatus.FAILURE:
                    case NodeStatus.SUCCESS:
                        // 次回の評価は先頭ノードから開始する
                        nodeContext.nodeIndex = 0;
                        break;
                    case NodeStatus.RUNNING:
                        // 次回の評価はこのノードから開始する
                        nodeContext.nodeIndex = i;
                        break;
                    default:
                        Debug.Assert(false, "Invalid Node Status");
                        break;
                }

                // FAILURE の場合のみ、次のノードを評価する
                if (nodeContext.nodeStatus != NodeStatus.FAILURE)
                {
                    break;
                }
            }

            context.dict.Store(GetInstanceID(), nodeContext);

#if UNITY_EDITOR
            m_nodeStatus = nodeContext.nodeStatus;
#endif

            return nodeContext.nodeStatus;
        }

        public override void Abort(BehaviourTreeContext context)
        {
            context.dict.Remove(GetInstanceID());
        }

#if UNITY_EDITOR
        public override NodeStatus GetStatus()
        {
            return m_nodeStatus;
        }
#endif

        protected class SelectorNodeContext : NodeContext
        {
            public int nodeIndex;

            public SelectorNodeContext()
            {
                nodeIndex = 0;
            }
        }
    }
}
