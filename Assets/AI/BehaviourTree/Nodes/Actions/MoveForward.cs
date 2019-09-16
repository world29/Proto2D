using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D.AI
{
    public class MoveForward : Action
    {
        public float speed = 1;

        public override NodeStatus Evaluate(EnemyBehaviour context)
        {
            context.MoveForward(speed);

            m_nodeStatus = NodeStatus.SUCCESS;

            return m_nodeStatus;
        }

        public override float GetProgress()
        {
            return 1;
        }
    }
}
