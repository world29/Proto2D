using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D.AI
{
    [CreateNodeMenu("BehaviourTree/Action/PlayAnimatorState")]
    public class PlayAnimatorState : Action
    {
        public string m_stateName;

        public override NodeStatus Evaluate(EnemyBehaviour enemyBehaviour)
        {
            Animator animator = enemyBehaviour.gameObject.GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogWarning("Animator is not set");
                return NodeStatus.FAILURE;
            }

            animator.Play(m_stateName);

            return NodeStatus.SUCCESS;
        }
    }
}
