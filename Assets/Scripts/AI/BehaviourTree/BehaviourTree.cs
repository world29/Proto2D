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

        // override XNode.Init()
        // MEMO: ノード (ScriptableObject) がインスタンス化されたとき呼ばれる。
        //       ノードがコピーされた場合、接続をつなぎなおす前に呼ばれるため、NodePort にアクセスすることは避ける。
        protected override void Init()
        {
            m_nodeStatus = NodeStatus.READY;
        }

        // ルートノードの判定
        public bool IsRoot()
        {
            XNode.NodePort inPort = GetInputPort("parent");
            return !inPort.IsConnected;
        }

        // BehaviourTree の使用を開始する前に一度だけ呼ばれる。
        // NodePort にアクセスする処理はここで行う。
        public virtual void Setup() { }

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
            return m_nodeStatus;
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

        public void Setup()
        {
            // 全ノードのセットアップ
            nodes.ForEach(node => (node as Node).Setup());

            // ルートノードを検索する
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
