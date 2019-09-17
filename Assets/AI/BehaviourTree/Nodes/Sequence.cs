using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D.AI
{
    public class Sequence : Node
    {
        [Output] public List<Node> children;

        private List<Node> m_nodes = new List<Node>();

#if UNITY_EDITOR
        private NodeStatus m_nodeStatus = NodeStatus.SUCCESS;
#endif

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
        }

        public override void PrepareForEvaluation(BehaviourTreeContext context)
        {
            SequenceNodeContext nodeContext = context.dict.Get<SequenceNodeContext>(GetInstanceID());
            if (nodeContext.nodeStatus != NodeStatus.RUNNING)
            {
                nodeContext.nodeStatus = NodeStatus.READY;
            }
        }

        public override NodeStatus Evaluate(BehaviourTreeContext context)
        {
            SequenceNodeContext nodeContext = context.dict.Get<SequenceNodeContext>(GetInstanceID());

            int i = nodeContext.nodeIndex;
            for (; i < m_nodes.Count; i++)
            {
                nodeContext.nodeStatus = m_nodes[i].Evaluate(context);
                switch (nodeContext.nodeStatus)
                {
                    case NodeStatus.FAILURE:
                        // 次回の評価は先頭ノードから開始する
                        nodeContext.nodeIndex = 0;
                        break;
                    case NodeStatus.RUNNING:
                        // 次回の評価はこのノードから開始する
                        nodeContext.nodeIndex = i;
                        break;
                    case NodeStatus.SUCCESS:
                        break;
                    default:
                        Debug.Assert(false, "Invalid Node Status");
                        break;
                }

                // SUCCESS の場合のみ、次のノードを評価する
                if (nodeContext.nodeStatus != NodeStatus.SUCCESS)
                {
                    break;
                }
            }

            // 末尾ノードが SUCCESS なら、次回の評価は先頭ノードから開始する
            if (i == m_nodes.Count && nodeContext.nodeStatus == NodeStatus.SUCCESS)
            {
                nodeContext.nodeIndex = 0;
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

        protected class SequenceNodeContext : NodeContext
        {
            public int nodeIndex;

            public SequenceNodeContext()
            {
                nodeIndex = 0;
            }
        }
    }
}
