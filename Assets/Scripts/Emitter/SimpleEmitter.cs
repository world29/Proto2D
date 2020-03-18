using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    public class SimpleEmitter : MonoBehaviour, IProjectileEmitter
    {
        [Header("発射物")]
        public Projectile m_projectile;

        [Header("発射位置。null ならこのコンポーネントがアタッチされているオブジェクトの位置。")]
        public Transform m_emitLocation;

        [Header("初速度")]
        public float m_speed;

        // IProjectileEmitter
        public void Emit()
        {
            Vector3 position = transform.position;
            
            if (m_emitLocation)
            {
                position = m_emitLocation.position;
            }

            var projectile = GameObject.Instantiate(m_projectile, position, Quaternion.identity) as Projectile;

            Vector3 direction = (transform.lossyScale.x >= 0) ? Vector3.right : Vector3.left;
            projectile.initialVelocity = transform.rotation * direction * m_speed;
        }
    }
}
