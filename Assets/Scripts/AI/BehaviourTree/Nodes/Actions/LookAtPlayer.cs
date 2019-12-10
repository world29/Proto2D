using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D.AI
{
    [CreateNodeMenu("BehaviourTree/Action/LookAtPlayer")]
    public class LookAtPlayer : Action
    {
        public string m_stateName;
        public override NodeStatus Evaluate(EnemyBehaviour enemyBehaviour)
        {
            Animator animator = enemyBehaviour.gameObject.GetComponent<Animator>();
            if (m_stateName != "" && animator && animator.isActiveAndEnabled)
            {
                animator.Play(m_stateName);
            }
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject)
            {
                enemyBehaviour.LookAt(playerObject.transform);
            }

            return NodeStatus.SUCCESS;
        }
    }
}
