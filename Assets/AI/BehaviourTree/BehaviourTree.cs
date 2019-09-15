using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D.AI
{
    public enum NodeStatus { READY, FAILURE, SUCCESS, RUNNING }

    public abstract class Node : XNode.Node
    {
        [Input] public Node parent;

        protected NodeStatus m_nodeStatus;

        public NodeStatus Status { get { return m_nodeStatus; } }

        public Node() { }

        public bool IsRoot()
        {
            XNode.NodePort inPort = GetInputPort("parent");
            return !inPort.IsConnected;
        }

        protected override void Init()
        {
            m_nodeStatus = NodeStatus.READY;
        }

        public virtual void Reset()
        {
            // 明示的に Init を呼ぶ
            Init();
        }

        public virtual void PreEvaluate()
        {
            if (m_nodeStatus != NodeStatus.RUNNING)
            {
                m_nodeStatus = NodeStatus.READY;
            }
        }

        public abstract NodeStatus Evaluate(EnemyBehaviour context);

        public override object GetValue(XNode.NodePort port)
        {
            return null;
        }
    }

    [CreateAssetMenu(fileName = "BehaviourTree", menuName = "Node Graph/BehaviourTree")]
    public class BehaviourTree : XNode.NodeGraph
    {
        private Node m_rootNode;

        // ルートノードを検索する
        // 最初に呼ぶ必要がある
        public void OnStart()
        {
            XNode.Node root = nodes.Find(item => (item as Node).IsRoot());

            m_rootNode = root as Node;
            Debug.Assert(m_rootNode != null, "BehaviourTree.m_rootNode is null");
        }

        public void OnRestart()
        {
            nodes.ForEach(item => (item as Node).Reset());
        }

        public NodeStatus OnUpdate(EnemyBehaviour context)
        {
            PrepareForEvaluate();

            return m_rootNode.Evaluate(context);
        }

        private void PrepareForEvaluate()
        {
            nodes.ForEach(item => (item as Node).PreEvaluate());
        }
    }
}
