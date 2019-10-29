using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Proto2D.AI
{
    public enum AbortType { None, LowerPriority }

    [NodeTint(0.7f, 0.7f, 1f)]
    public abstract class Composite : Node
    {
        public AbortType m_abortType = AbortType.None;

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

        protected override void OnReady()
        {
            m_nodeIndex = 0;
        }

        protected override void OnAbort()
        {
            if (m_nodeStatus == NodeStatus.RUNNING)
            {
                m_nodes[m_nodeIndex].Abort();
            }
        }

        public override void CollectConditionals(ref List<Conditional> conditionalNodes)
        {
            foreach (var node in m_nodes)
            {
                node.CollectConditionals(ref conditionalNodes);
            }
        }

        protected void ReevaluateConditionals(EnemyBehaviour enemyBehaviour)
        {
            if (m_abortType == AbortType.None)
            {
                return;
            }

            if (m_nodeStatus != NodeStatus.RUNNING)
            {
                return;
            }

            // 現在実行中のノードよりも優先度の高いノードを再評価する
            // 状態が変化していたら現在実行中のノードを中断して、変化したノードから評価を再開する。
            List<Node> higherPriorityNodes = m_nodes.Where((node, i) => (i < m_nodeIndex)).ToList();

            bool flag = false;
            foreach (var node in higherPriorityNodes)
            {
                List<Conditional> conditionals = new List<Conditional>();
                node.CollectConditionals(ref conditionals);

                foreach (var cond in conditionals)
                {
                    NodeStatus prevStatus = cond.PrevStatus;
                    if (prevStatus != cond.Evaluate(enemyBehaviour))
                    {
                        m_nodes[m_nodeIndex].Abort();
                        m_nodeIndex = m_nodes.FindIndex(nd => nd == node);
                        flag = true;
                        break;
                    }
                }

                if (flag) break;
            }
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
    }
}
