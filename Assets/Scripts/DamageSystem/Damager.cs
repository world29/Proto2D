using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Proto2D
{
    [RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
    public class Damager : MonoBehaviour
    {
        public DamageType m_damageType = DamageType.Attack;

        public float damage = 1;

        public GameObject sender;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            ProcessTrigger(collision);
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            ProcessTrigger(collision);
        }

        void ProcessTrigger(Collider2D collision)
        {
            if (!enabled) return;

            Damageable damageable = collision.GetComponent<Damageable>();
            if (damageable && damageable.enabled)
            {
                DamageTypeFlag flag = (DamageTypeFlag)(0x1 << (int)m_damageType);
                if ((flag & damageable.m_damageTypeFlag) == 0)
                {
                    return;
                }

                GameObject receiver = damageable.m_receiver;

                // 攻撃がヒットしたことを相手に通知
                ExecuteEvents.Execute<IDamageReceiver>(receiver, null,
                    (target, eventTarget) => target.OnReceiveDamage(m_damageType, damage, gameObject));

                // 攻撃がヒットしたことを自分自身に通知
                ExecuteEvents.Execute<IDamageSender>(sender, null,
                    (target, eventTarget) => target.OnApplyDamage(m_damageType, damage, receiver));
            }
        }

        private void OnDrawGizmos()
        {
            if (!enabled) return;

            BoxCollider2D collider = GetComponent<BoxCollider2D>();
            if (collider.enabled)
            {
                Gizmos.color = new Color(1, 0, 0, .2f);
                Gizmos.DrawCube(collider.bounds.center, collider.bounds.size);
            }
        }
    }
}
