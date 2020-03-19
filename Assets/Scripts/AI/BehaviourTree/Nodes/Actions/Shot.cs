using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D.AI
{
    [CreateNodeMenu("BehaviourTree/Action/Shot")]
    public class Shot : Action
    {
        public string m_stateName;

        public override NodeStatus Evaluate(EnemyBehaviour enemyBehaviour)
        {
            Animator animator = enemyBehaviour.gameObject.GetComponent<Animator>();
            if (m_stateName != "" && animator && animator.isActiveAndEnabled)
            {
                animator.Play(m_stateName);
            }
            var emitter = enemyBehaviour.gameObject.GetComponent<IProjectileEmitter>() as IProjectileEmitter;
            if (emitter != null) {
              emitter.Emit();
            }

            return NodeStatus.SUCCESS;
        }
    }
}
