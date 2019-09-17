using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D.AI
{
    public class MoveForward : Action
    {
        public float speed = 1;

        public override NodeStatus Evaluate(BehaviourTreeContext context)
        {
            context.enemy.MoveForward(speed);

            return NodeStatus.SUCCESS;
        }

        public override float GetProgress()
        {
            return 1;
        }
    }
}
