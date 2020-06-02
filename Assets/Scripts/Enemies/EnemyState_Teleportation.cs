using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Proto2D
{
    public class EnemyState_Teleportation : IEnemyState
    {
        private List<Damager> m_damagersCache = new List<Damager>();

        private Animator m_animator;
        private Vector3 m_targetPosition;
        private Vector3 m_velocity;
        private float m_smoothTime = .3f;

        public void OnEnter(EnemyBehaviour context)
        {
            m_animator = context.gameObject.GetComponent<Animator>();

            // AI キャンセル
            if (context.behaviourTree)
            {
                context.behaviourTree.Abort();
            }

            m_animator.SetBool("teleportation", true);

            disableCollisions(context);

            // Controller2D による座標更新を停止する
            context.suspended = true;

            // テレポート先の座標を取得
            TilemapController tc = GameObject.FindObjectOfType<TilemapController>();
            if (tc)
            {
                Vector3[] positions = tc.QueryPositionsForTeleportation();
                if (positions.Count() > 0)
                {
                    // ビューポート内でもっとも高い位置にある座標を選択する
                    m_targetPosition = positions.Aggregate((acc, cur) => {
                        bool isInViewport = Camera.main.WorldToViewportPoint(cur).y < 1;
                        if (isInViewport && acc.y < cur.y)
                        {
                            return cur;
                        }
                        return acc;
                    });
                }
            }

            m_velocity = Vector3.zero;
        }

        public void OnExit(EnemyBehaviour context)
        {
            context.suspended = false;
            m_animator.SetBool("teleportation", false);
            enableCollisions(context);
        }

        public IEnemyState OnUpdate(EnemyBehaviour context)
        {
            Vector3.SmoothDamp(context.transform.position, m_targetPosition, ref m_velocity, m_smoothTime);

            context.transform.Translate(m_velocity * Time.deltaTime);

            // 十分近づいたらステート遷移
            if (Vector3.Distance(context.transform.position, m_targetPosition) < 1)
            {
                return new EnemyState_Idle();
            }

            return this;
        }

        void disableCollisions(EnemyBehaviour context)
        {
            Damager[] damagers = context.GetComponentsInChildren<Damager>();
            foreach (var damager in damagers)
            {
                if (damager.enabled)
                {
                    m_damagersCache.Add(damager);
                    damager.enabled = false;
                }
            }

            Damageable[] damageables = context.GetComponentsInChildren<Damageable>();
            foreach (var damageable in damageables)
            {
                damageable.enabled = false;
            }
        }

        void enableCollisions(EnemyBehaviour context)
        {
            foreach (var damager in m_damagersCache)
            {
                damager.enabled = true;
            }

            Damageable[] damageables = context.GetComponentsInChildren<Damageable>();
            foreach (var damageable in damageables)
            {
                damageable.enabled = true;
            }
        }
    }
}
