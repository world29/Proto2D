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

        // コンストラクタ
        public Node()
        {
            m_nodeStatus = NodeStatus.READY;
        }

        // ルートノードの判定
        public bool IsRoot()
        {
            XNode.NodePort inPort = GetInputPort("parent");
            return !inPort.IsConnected;
        }

        // Evaluate() の前に呼ばれ、RUNNING じゃないノードのステータスを READY にする
        public virtual void ResetStatus()
        {
            if (m_nodeStatus != NodeStatus.RUNNING)
            {
                m_nodeStatus = NodeStatus.READY;
            }
        }

        // ノードの評価
        public abstract NodeStatus Evaluate(EnemyBehaviour enemyBehaviour);

        // 中断
        public virtual void Abort()
        {
            m_nodeStatus = NodeStatus.READY;
        }

        // ステータスの取得
        public virtual NodeStatus GetStatus()
        {
            return NodeStatus.SUCCESS;
        }

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
        public void Init()
        {
            if (m_rootNode == null)
            {
                XNode.Node root = nodes.Find(item => (item as Node).IsRoot());

                m_rootNode = root as Node;
                Debug.Assert(m_rootNode != null, "BehaviourTree.m_rootNode is null");
            }
        }

        public NodeStatus Evaluate(EnemyBehaviour enemyBehaviour)
        {
            nodes.ForEach(item => {
                Debug.Assert(item is Node);
                (item as Node).ResetStatus();
            });

            return m_rootNode.Evaluate(enemyBehaviour);
        }

        public void Abort()
        {
            nodes.ForEach(item => (item as Node).Abort());
        }
    }
}
