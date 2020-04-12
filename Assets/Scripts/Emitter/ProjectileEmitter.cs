using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Proto2D
{
    public class ProjectileEmitter : MonoBehaviour, IEmitter
    {
        [Header("発射物")]
        public Projectile m_projectile;

        [Header("発射位置。null ならこのコンポーネントがアタッチされているオブジェクトの位置。")]
        public Transform m_locator;

        [Header("初速度")]
        public float m_speed;

        // IEmitter
        public float Speed { get { return m_speed; } set { m_speed = value; } }

        // IEmitter
        [ContextMenu("Emit")]
        public void Emit()
        {
            Vector3 position = transform.position;
            Quaternion rotation = transform.rotation;
            var speed = m_speed;

            if (m_locator)
            {
                position = m_locator.position;
                rotation = m_locator.rotation;
            }

            // 向きの反転を反映
            var q = rotation;
            if (transform.lossyScale.x < 0)
            {
                speed *= -1;
                //var e = rotation.eulerAngles;
                //q = Quaternion.Euler(e.x, e.y, 180 - e.z);
            }

            var projectile = GameObject.Instantiate(m_projectile, position, q) as Projectile;

            if (transform.lossyScale.x < 0)
            {
                Vector3 p_scale = projectile.transform.localScale;
                projectile.transform.localScale = new Vector3(p_scale.x *= -1* transform.localScale.x, p_scale.y* transform.localScale.y, p_scale.z* transform.localScale.z);
            }
            else
            {
                projectile.transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }

            // 初速を計算
            Vector3 initialVelocity = q * Vector3.right * speed;

            projectile.rigidbody.velocity = initialVelocity;

            Debug.DrawLine(position, position + initialVelocity);
        }
    }
}
