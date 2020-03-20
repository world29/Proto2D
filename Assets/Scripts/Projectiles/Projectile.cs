using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Proto2D
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collision2D))]
    public class Projectile : MonoBehaviour
    {
        // 非推奨な UnityEngine.Component.rigidbody を無効化するため new で宣言
        [HideInInspector]
        public new Rigidbody2D rigidbody { get { return GetComponent<Rigidbody2D>(); } }

        [SerializeField, Header("ヒットイベント (プレイヤー)")]
        UnityEvent m_OnHit;

        [SerializeField, Header("衝突するレイヤー")]
        LayerMask m_collisionMask;

        [SerializeField, Header("衝突イベント (Collision Mask で指定したレイヤー)")]
        UnityEvent m_OnCollision;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                m_OnHit.Invoke();

                Destroy(gameObject);
            }
            else if (m_collisionMask == (m_collisionMask | (1 << collision.gameObject.layer)))
            {
                m_OnCollision.Invoke();

                Destroy(gameObject);
            }
        }
    }
}
