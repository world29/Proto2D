using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Proto2D
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class Damager : MonoBehaviour
    {
        [SerializeField, Header("ダメージを与えるレイヤー")]
        LayerMask m_layerMask;

        [SerializeField, Header("ダメージタイプ")]
        public DamageType m_damageType = DamageType.Contact;

        [SerializeField, Header("ダメージ量")]
        float m_damageAmount = 1;

        [SerializeField, Header("ダメージを与えたことを伝えるオブジェクト。null の場合はルートオブジェクト")]
        GameObject m_owner;

        private void Awake()
        {
            if (m_owner == null)
            {
                m_owner = transform.root.gameObject;
            }

            // sender が IDamageSender を実装していない
            if (m_owner.GetComponent<IDamageSender>() == null)
            {
                Debug.LogWarningFormat("{0} can not receive DamageApplyEvent", m_owner.name);
            }

            //TODO: Damager 仕様変更への一時対応
            if (m_layerMask == 0)
            {
                m_layerMask = LayerMask.NameToLayer("Player");
            }
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            OnHit(collider);
        }

        private void OnTriggerStay2D(Collider2D collider)
        {
            OnHit(collider);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            OnHit(collision.collider);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            OnHit(collision.collider);
        }

        void OnHit(Collider2D collider)
        {
            // このコンポーネントが無効
            if (!enabled) return;

            // ルートが同じ (自分自身)
            if (collider.transform.root == transform.root) return;

            // レイヤー判定
            if ((m_layerMask & (0x1 << collider.gameObject.layer)) == 0) return;

            // ダメージを適用
            Damageable damageable = collider.GetComponent<Damageable>();
            if (damageable && damageable.enabled)
            {
                // ダメージタイプの不一致
                DamageTypeFlag damageFlag = (DamageTypeFlag)(0x1 << (int)m_damageType);
                if ((damageable.m_damageTypeFlag & damageFlag) == 0)
                {
                    return;
                }

                // ダメージを受けたことを相手に通知
                Debug.Assert(damageable.m_owner != null);
                ExecuteEvents.Execute<IDamageReceiver>(damageable.m_owner, null,
                    (target, eventTarget) => target.OnReceiveDamage(m_damageType, m_damageAmount, m_owner));

                // ダメージを与えたしたことを自身に通知
                ExecuteEvents.Execute<IDamageSender>(m_owner, null,
                    (target, eventTarget) => target.OnApplyDamage(m_damageType, m_damageAmount, damageable.m_owner));
            }
        }

        private void OnDrawGizmos()
        {
            if (!enabled) return;

            // ダメージ判定の矩形描画
            Collider2D collider = GetComponent<Collider2D>();
            if (collider.enabled)
            {
                Gizmos.color = new Color(1, 0, 0, .2f);
                Gizmos.DrawCube(collider.bounds.center, collider.bounds.size);
            }
        }
    }
}
