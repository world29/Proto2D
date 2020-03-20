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
        public void Emit()
        {
            Vector3 position = transform.position;
            Quaternion rotation = transform.rotation;

            if (m_locator)
            {
                position = m_locator.position;
                rotation = m_locator.rotation;
            }

            var projectile = GameObject.Instantiate(m_projectile, position, rotation) as Projectile;

            // 初速を計算
            Vector3 direction = (transform.lossyScale.x >= 0) ? Vector3.right : Vector3.left;
            Vector3 initialVelocity = rotation * direction * m_speed;

            projectile.rigidbody.velocity = initialVelocity;

            Debug.DrawLine(position, position + initialVelocity);
        }
    }
}
