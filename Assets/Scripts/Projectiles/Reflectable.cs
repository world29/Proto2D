using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UniRx;
using UniRx.Triggers;

namespace Proto2D
{
    public class Reflectable : Projectile, IDamageSender
    {
        protected override void OnHit(Collider2D collider)
        {
            if ((m_hitMask & (0x1 << collider.gameObject.layer)) != 0)
            {
                m_OnHit.Invoke();

                // 向きと速度を反転
                {
                    transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);

                    Vector3 reflectedVelocity = rigidbody.velocity;
                    reflectedVelocity.x *= -1;
                    reflectedVelocity.y *= .5f;
                    rigidbody.velocity = reflectedVelocity;
                }
            }
        }

        public void OnApplyDamage(DamageType type, float damage, GameObject receiver)
        {
            // ダメージを与えたら自身を削除する
            base.Kill();
        }
    }
}
