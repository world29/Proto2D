using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D.AI
{
    [CreateNodeMenu("BehaviourTree/Action/LookAtPlayer")]
    public class LookAtPlayer : Action
    {
        public override NodeStatus Evaluate(EnemyBehaviour enemyBehaviour)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject)
            {
                enemyBehaviour.LookAt(playerObject.transform);
            }

            return NodeStatus.SUCCESS;
        }
    }
}
