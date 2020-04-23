using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Proto2D
{
    public class EnemyState_Damage : IEnemyState
    {
        private Animator m_animator;

        private float timeToTransition;

        private List<Damager> m_damagers = new List<Damager>();

        public void OnEnter(EnemyBehaviour enemyBehaviour)
        {
            m_animator = enemyBehaviour.gameObject.GetComponent<Animator>();

            // スーパーアーマーでない場合は、被ダメージ時に、AI 停止、与ダメージを無効化
            if (!enemyBehaviour.superArmor || enemyBehaviour.health == 0)
            {
                if (enemyBehaviour.behaviourTree)
                {
                    enemyBehaviour.behaviourTree.Abort();
                }

                Damager[] damagers = enemyBehaviour.GetComponentsInChildren<Damager>();
                foreach (var damager in damagers)
                {
                    if (damager.enabled)
                    {
                        m_damagers.Add(damager);
                        damager.enabled = false;
                    }
                }
                m_animator.SetBool("damage", true);


            }
            float blink = enemyBehaviour.damageDuration;
            float interval = enemyBehaviour.blinkInterval;
            if (enemyBehaviour.health == 0)
            {
                m_animator.SetBool("damage", false);
                m_animator.SetBool("death", true);
                blink = enemyBehaviour.deathDuration;
                interval = enemyBehaviour.deathBlinkInterval;
            }

            enemyBehaviour.PlaySE(enemyBehaviour.damageSE);
            enemyBehaviour.PlayEffect(enemyBehaviour.damageEffectPrefab);

            enemyBehaviour.Blink(blink, interval);

            // スーパーアーマーに関わらずダメージ中は被ダメージを停止
            // 連続でダメージが入らないようにするため
            Damageable[] damageables = enemyBehaviour.GetComponentsInChildren<Damageable>();
            foreach (var damageable in damageables)
            {
                damageable.enabled = false;
            }

            timeToTransition = Time.timeSinceLevelLoad + enemyBehaviour.damageDuration;
        }

        public void OnExit(EnemyBehaviour enemyBehaviour)
        {
            if (!enemyBehaviour.superArmor)
            {
                m_animator.SetBool("damage", false);

                foreach (var damager in m_damagers)
                {
                    damager.enabled = true;
                }
            }

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
