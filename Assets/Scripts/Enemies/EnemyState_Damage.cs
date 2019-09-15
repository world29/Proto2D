using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    public class EnemyState_Damage : IEnemyState
    {
        private Animator animator;

        private float timeToTransition;

        public void OnEnter(EnemyBehaviour context)
        {
            animator = context.gameObject.GetComponent<Animator>();

            animator.SetBool("damage", true);

            context.Blink(context.damageDuration, context.blinkInterval);

            timeToTransition = Time.timeSinceLevelLoad + context.damageDuration;
        }

        public void OnExit(EnemyBehaviour context)
        {
            animator.SetBool("damage", false);
        }

        public IEnemyState OnUpdate(EnemyBehaviour context)
        {
            if (Time.timeSinceLevelLoad >= timeToTransition)
            {
                return new EnemyState_Idle();
            }

            return this;
        }
    }
}
