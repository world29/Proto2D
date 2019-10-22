using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D.AI
{
    public abstract class Action : Node
    {
        public virtual float GetProgress()
        {
            return 0;
        }
    }
}
