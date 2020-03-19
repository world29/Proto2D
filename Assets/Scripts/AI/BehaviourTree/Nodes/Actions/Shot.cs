using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D.AI
{
    [CreateNodeMenu("BehaviourTree/Action/Shot")]
    public class Shot : Action
    {
        public string m_stateName;
        public Projectile projectilePrefab;

        public override NodeStatus Evaluate(EnemyBehaviour enemyBehaviour)
        {
            Animator animator = enemyBehaviour.gameObject.GetComponent<Animator>();
            if (m_stateName != "" && animator && animator.isActiveAndEnabled)
            {
                animator.Play(m_stateName);
            }
            enemyBehaviour.Shot(projectilePrefab);

            return NodeStatus.SUCCESS;
        }
    }
}
