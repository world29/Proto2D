using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D.AI
{
    [CreateNodeMenu("BehaviourTree/Decorator/ConditionalLoop")]
    public class ConditionalLoop : Decorator
    {
        [Tooltip("接続した Conditional ノードが SUCCESS である間、実行し続けます。"), Output(ShowBackingValue.Never, ConnectionType.Override)] public Node condition;

        protected Node m_conditionNode;

        public override void Setup()
        {
            base.Setup();

            XNode.NodePort outPort = GetOutputPort("condition");
            if (!outPort.IsConnected)
            {
                return;
            }

            m_conditionNode = outPort.Connection.node as Node;
        }

        public override NodeStatus Evaluate(EnemyBehaviour enemyBehaviour)
        {
            if (m_conditionNode.Evaluate(enemyBehaviour) == NodeStatus.SUCCESS)
            {
                m_node.Evaluate(enemyBehaviour);

                // 子ノードの実行結果によらず、RUNNING ステータスとする
                m_nodeStatus = NodeStatus.RUNNING;
            }
            else
            {
                m_nodeStatus = NodeStatus.FAILURE;
            }

            return m_nodeStatus;
        }
    }
}
