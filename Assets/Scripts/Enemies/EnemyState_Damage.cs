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
            if (!enemyBehaviour.superArmor)
            {
                if (enemyBehaviour.behaviourTree)
                {
                    enemyBehaviour.behaviourTree.Abort();
                }
            }

            animator = enemyBehaviour.gameObject.GetComponent<Animator>();

            animator.SetBool("damage", true);
            
            enemyBehaviour.PlaySE(enemyBehaviour.damageSE);
            enemyBehaviour.PlayEffect(enemyBehaviour.damageEffectPrefab);

            enemyBehaviour.Blink(enemyBehaviour.damageDuration, enemyBehaviour.blinkInterval);

            Damageable[] damageables = enemyBehaviour.GetComponentsInChildren<Damageable>();
            foreach (var damageable in damageables)
            {
                damageable.enabled = false;
            }

            timeToTransition = Time.timeSinceLevelLoad + enemyBehaviour.damageDuration;
        }

        public void OnExit(EnemyBehaviour enemyBehaviour)
        {
            animator.SetBool("damage", false);

            Damageable[] damageables = enemyBehaviour.GetComponentsInChildren<Damageable>();
            foreach (var damageable in damageables)
            {
                damageable.enabled = true;
            }
        }

        public IEnemyState OnUpdate(EnemyBehaviour enemyBehaviour)
        {
            if (Time.timeSinceLevelLoad >= timeToTransition)
            {
                if (enemyBehaviour.health == 0)
                {
                    //MEMO: 仮実装として SetActive(false) している
                    enemyBehaviour.gameObject.SetActive(false);
                }
                return new EnemyState_Idle();
            }

            if (enemyBehaviour.superArmor)
            {
                enemyBehaviour.UpdateBehaviourTree();
            }

            return this;
        }
    }
}
