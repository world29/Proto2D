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

        public virtual void PreEvaluate()
        {
            if (m_nodeStatus != NodeStatus.RUNNING)
            {
                m_nodeStatus = NodeStatus.READY;
            }
        }

        public abstract NodeStatus Evaluate(BehaviourContext context);

        public override object GetValue(XNode.NodePort port)
        {
            return null;
        }
    }

    public struct BehaviourContext
    {
        public GameObject gameObject;
    }

    [CreateAssetMenu(fileName = "BehaviourTree", menuName = "Node Graph/BehaviourTree")]
    public class BehaviourTree : XNode.NodeGraph
    {
        private Node m_rootNode;

        // ルートノードを検索する
        // 最初に呼ぶ必要がある
        public void Init()
        {
            Debug.Log("BehaviourTree.Init() called");

            XNode.Node root = nodes.Find(item => (item as Node).IsRoot());

            m_rootNode = root as Node;
            Debug.Assert(m_rootNode != null, "BehaviourTree.m_rootNode is null");
        }

        public NodeStatus Evaluate(GameObject gameObject)
        {
            ResetNodeStatusAll();

            BehaviourContext context;
            context.gameObject = gameObject;

            return m_rootNode.Evaluate(context);
        }

        public void ResetNodeStatusAll()
        {
            nodes.ForEach(item => (item as Node).PreEvaluate());
        }
    }
}
