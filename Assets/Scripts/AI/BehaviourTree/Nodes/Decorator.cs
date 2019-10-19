﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D.AI
{
    public abstract class Decorator : Node
    {
        [Output] public Node child;

        [SerializeField] protected Node m_node;

        // override XNode.Init()
        protected override void Init()
        {
            base.Init();

            XNode.NodePort outPort = GetOutputPort("child");
            if (!outPort.IsConnected)
            {
                return;
            }

            m_node = outPort.Connection.node as Node;
        }
    }
}
