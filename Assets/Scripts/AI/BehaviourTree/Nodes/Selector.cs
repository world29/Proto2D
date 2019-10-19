﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D.AI
{
    [CreateNodeMenu("BehaviourTree/Selector")]
    public class Selector : Node
    {
        [Output] public List<Node> children;

        private List<Node> m_nodes = new List<Node>();

        private int m_nodeIndex;

        // override XNode.Init()
        protected override void Init()
        {
            base.Init();

            m_nodeIndex = 0;
        }

        public override void Setup()
        {
            base.Setup();

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
