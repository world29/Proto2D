using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D.AI
{
    [NodeTint(1f, 0.5f, 0.5f)]
    public abstract class Decorator : Node
    {
        [Output(ShowBackingValue.Never, ConnectionType.Override)] public Node child;

        protected Node m_node;

        // override XNode.Init()
        protected override void Init()
        {
            base.Init();
        }

        public override void Setup()
        {
            base.Setup();

            XNode.NodePort outPort = GetOutputPort("child");
            if (!outPort.IsConnected)
            {
                return;
            }

            m_node = outPort.Connection.node as Node;
        }

        protected override void OnAbort()
        {
            m_node.Abort();
        }

        public override void CollectConditionals(ref List<Conditional> conditionalNodes)
        {
            m_node.CollectConditionals(ref conditionalNodes);
        }
    }
}
