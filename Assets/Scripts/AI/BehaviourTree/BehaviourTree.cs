using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D.AI
{
    public enum NodeStatus { READY, FAILURE, SUCCESS, RUNNING }

    public class BehaviourTreeContext
    {
        public EnemyBehaviour enemy;
        public NodeContextDictionary dict;

        public BehaviourTreeContext(EnemyBehaviour _enemy)
        {
            enemy = _enemy;
            dict = new NodeContextDictionary();
        }

        public class NodeContextDictionary
        {
            private Dictionary<int, object> dict = new Dictionary<int, object>();

            public void Store<T>(int key, T context)
            {
                Remove(key);
                dict.Add(key, context);
            }

            public T Get<T>(int key) where T : new()
            {
                if (!dict.ContainsKey(key))
                {
                    dict.Add(key, new T());
                }
                return (T)dict[key];
            }

            public void Remove(int key)
            {
                if (dict.ContainsKey(key))
                {
                    dict.Remove(key);
                }
            }
        }
    }

    public abstract class Node : XNode.Node
    {
        [Input] public Node parent;

        public Node() { }

        public bool IsRoot()
        {
            XNode.NodePort inPort = GetInputPort("parent");
            return !inPort.IsConnected;
        }

        public virtual void PrepareForEvaluation(BehaviourTreeContext context) { }

        public abstract NodeStatus Evaluate(BehaviourTreeContext context);

        public virtual void Abort(BehaviourTreeContext context) { }

#if UNITY_EDITOR
        public virtual NodeStatus GetStatus()
        {
            return NodeStatus.SUCCESS;
        }
#endif

        public override object GetValue(XNode.NodePort port)
        {
            return null;
        }

        protected class NodeContext {
            public NodeStatus nodeStatus;

            public NodeContext()
            {
                nodeStatus = NodeStatus.READY;
            }
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

        public NodeStatus Evaluate(BehaviourTreeContext context)
        {
            nodes.ForEach(item => {
                Debug.Assert(item is Node);
                (item as Node).PrepareForEvaluation(context);
            });

            return m_rootNode.Evaluate(context);
        }

        public void Abort(BehaviourTreeContext context)
        {
            nodes.ForEach(item => (item as Node).Abort(context));
        }
    }
}
