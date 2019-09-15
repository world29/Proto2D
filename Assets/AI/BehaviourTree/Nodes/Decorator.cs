using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D.AI
{
    public abstract class DecoratorNode : Node
    {
        [Output] public Node child;
    }
}
