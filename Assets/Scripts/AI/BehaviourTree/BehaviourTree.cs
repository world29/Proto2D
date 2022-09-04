using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Proto2D.AI
{
    public enum NodeStatus { READY, FAILURE, SUCCESS, RUNNING, ABORTED }

    public abstract class Node : XNode.Node
    {
        [Input(ShowBackingValue.Never, ConnectionType.Override)] public Node parent;

        public NodeStatus Status { get { return m_nodeStatus; } }
        public NodeStatus PrevStatus { get { return getPreviousStatus(); } }
        public int Priority { get { return m_priority; } }
        public int EvaluationOrder { get { return m_evaluationOrder; } set { m_evaluationOrder = value; } }

        protected NodeStatus m_nodeStatus;
        private NodeStatus m_prevStatus;
        public int m_priority; // 個々のノードが持つ優先度
        protected int m_evaluationOrder; // コンポジット系ノードにおける評価順 (優先度に応じて決定される)

        // override XNode.Init()
        // MEMO: ノード (ScriptableObject) がインスタンス化されたとき呼ばれる。
        //       ノードがコピーされた場合、接続をつなぎなおす前に呼ばれるため、NodePort にアクセスすることは避ける。
        protected override void Init()
        {
            // 初回に OnReady が呼ばれて欲しいので初期値を SUCCESS or FAILURE にしておく
            m_nodeStatus = NodeStatus.SUCCESS;
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

        // ノードの状態が READY にリセットされた際に呼ばれる。
        // 乱数の生成など、ノード評価のたびに実行したい処理を記述する。
        protected virtual void OnReady() { }

        // ノードの実行が中断された際に呼ばれる。
        protected virtual void OnAbort() { }

        // ノードの評価
        public abstract NodeStatus Evaluate(EnemyBehaviour enemyBehaviour);

        // Conditional Abort の対象となる Conditional ノードを集める
        // Conditional ノード再評価のタイミングで Composite 系ノードから子ノードに対して再帰的に呼ばれる。
        public virtual void CollectConditionals(ref List<Conditional> conditionalNodes) { }

        // Evaluate() の前に呼ばれ、RUNNING じゃないノードのステータスを READY にする
        public void ResetStatus()
        {
            if (m_nodeStatus == NodeStatus.READY) return;

            if (m_nodeStatus != NodeStatus.RUNNING)
            {
                // デバッグ用に直前の評価結果を保存しておく
                m_prevStatus = m_nodeStatus;
                m_nodeStatus = NodeStatus.READY;

                OnReady();
            }
        }

        // 中断
        public void Abort()
        {
            m_nodeStatus = NodeStatus.ABORTED;

            OnAbort();
        }

        private NodeStatus getPreviousStatus()
        {
            return m_nodeStatus != NodeStatus.READY ? m_nodeStatus : m_prevStatus;
        }

        public override object GetValue(XNode.NodePort port)
        {
            return null;
        }

        public override void OnRemoveConnection(XNode.NodePort port)
        {
            if (Inputs.Contains(port))
            {
                m_evaluationOrder = -1;
            }
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
            XNode.Node root = nodes.Find(item => (item as Node).IsRoot());

            m_rootNode = root as Node;
            Debug.Assert(m_rootNode != null, "BehaviourTree.m_rootNode is null");
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
