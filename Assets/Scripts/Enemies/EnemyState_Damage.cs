using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    public class EnemyState_Damage : IEnemyState
    {
        private Animator animator;

        private float timeToTransition;

        public void OnEnter(EnemyBehaviour enemyBehaviour)
        {
            animator = enemyBehaviour.gameObject.GetComponent<Animator>();

            animator.SetBool("damage", true);

            enemyBehaviour.Blink(enemyBehaviour.damageDuration, enemyBehaviour.blinkInterval);

            timeToTransition = Time.timeSinceLevelLoad + enemyBehaviour.damageDuration;
        }

        public void OnExit(EnemyBehaviour enemyBehaviour)
        {
            animator.SetBool("damage", false);
        }

        public IEnemyState OnUpdate(EnemyBehaviour enemyBehaviour)
        {
            if (Time.timeSinceLevelLoad >= timeToTransition)
            {
                return new EnemyState_Idle();
            }

            return this;
        }
    }
}
