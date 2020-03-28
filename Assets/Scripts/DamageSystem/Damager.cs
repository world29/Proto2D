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

        public DamageType m_damageType = DamageType.Contact;

        [SerializeField, Header("ダメージ量")]
        public float damage = 1;

        [SerializeField, Header("ダメージを与えたことを伝えるオブジェクト。null の場合はルートオブジェクト")]
        public GameObject sender;

        private void Awake()
        {
            if (sender == null)
            {
                sender = transform.root.gameObject;
            }

            // sender が IDamageSender を実装していない
            if (sender.GetComponent<IDamageSender>() == null)
            {
                Debug.LogWarningFormat("{0} can not receive DamageApplyEvent", sender.name);
            }
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            ProcessTrigger(collider);
        }

        private void OnTriggerStay2D(Collider2D collider)
        {
            ProcessTrigger(collider);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            ProcessTrigger(collision.collider);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            ProcessTrigger(collision.collider);
        }

        void ProcessTrigger(Collider2D collider)
        {
            // このコンポーネントが無効
            if (!enabled) return;

            // ルートが同じ (自分自身)
            if (collider.transform.root == transform.root) return;

            // レイヤー判定
            //NOTE: 既存の動作を変えないため、デフォルトなら従来と同じ動作にしておく
            if (m_layerMask.value != 0)
            {
                if ((m_layerMask & (0x1 << collider.gameObject.layer)) == 0) return;
            }

            // ダメージを適用
            Damageable damageable = collider.GetComponent<Damageable>();
            if (damageable && damageable.enabled)
            {
                // ダメージタイプの不一致
                DamageTypeFlag flag = (DamageTypeFlag)(0x1 << (int)m_damageType);
                if ((flag & damageable.m_damageTypeFlag) == 0)
                {
                    return;
                }

                GameObject receiver = damageable.m_receiver;

                // 攻撃がヒットしたことを相手に通知
                ExecuteEvents.Execute<IDamageReceiver>(receiver, null,
                    (target, eventTarget) => target.OnReceiveDamage(m_damageType, damage, sender));

                // 攻撃がヒットしたことを自分自身に通知
                ExecuteEvents.Execute<IDamageSender>(sender, null,
                    (target, eventTarget) => target.OnApplyDamage(m_damageType, damage, receiver));
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
