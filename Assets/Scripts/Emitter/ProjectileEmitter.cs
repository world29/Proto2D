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

            if (m_locator)
            {
                position = m_locator.position;
                rotation = m_locator.rotation;
            }

            // 向きの反転を反映
            var q = rotation;
            if (transform.lossyScale.x < 0)
            {
                var e = rotation.eulerAngles;
                q = Quaternion.Euler(e.x, e.y, 180 - e.z);
            }

            var projectile = GameObject.Instantiate(m_projectile, position, q) as Projectile;

            // 初速を計算
            Vector3 initialVelocity = q * Vector3.right * m_speed;

            projectile.rigidbody.velocity = initialVelocity;

            Debug.DrawLine(position, position + initialVelocity);
        }
    }
}
