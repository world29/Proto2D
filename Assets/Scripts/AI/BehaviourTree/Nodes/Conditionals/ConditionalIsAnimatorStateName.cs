using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D.AI
{
    [CreateNodeMenu("BehaviourTree/Conditional/IsAnimatorStateName")]
    public class ConditionalIsAnimatorStateName : Conditional
    {
        public string stateName = "";

        public override NodeStatus Evaluate(EnemyBehaviour enemyBehaviour)
        {
            m_nodeStatus = enemyBehaviour.IsAnimatorStateName(stateName)
                            ? NodeStatus.SUCCESS
                            : NodeStatus.FAILURE;

            return m_nodeStatus;
        }
    }
}
