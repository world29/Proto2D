using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D.AI
{
    [CreateNodeMenu("BehaviourTree/Action/Turn")]
    public class Turn : Action
    {
        public override NodeStatus Evaluate(EnemyBehaviour enemyBehaviour)
        {
            enemyBehaviour.Turn();

            return NodeStatus.SUCCESS;
        }
    }
}
