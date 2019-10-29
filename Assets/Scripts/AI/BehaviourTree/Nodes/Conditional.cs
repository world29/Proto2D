using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D.AI
{
    [NodeTint(1f, 1f, 0f)]
    public abstract class Conditional : Node
    {
        public override void CollectConditionals(ref List<Conditional> conditionalNodes)
        {
            conditionalNodes.Add(this);
        }
    }
}
