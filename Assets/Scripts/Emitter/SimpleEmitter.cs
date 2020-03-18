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

        [Header("デフォルト速度")]
        public float m_defaultSpeed;

        [Header("デフォルト仰角"), Range(-180, 180)]
        public int m_defaultElevation;

        // IProjectileEmitter
        public void Emit(float speed, int elevation)
        {
            Vector3 position = transform.position;
            
            if (m_emitLocation)
            {
                position = m_emitLocation.position;
            }

            var projectile = GameObject.Instantiate(m_projectile, position, Quaternion.identity) as Projectile;

            Vector3 direction = (transform.lossyScale.x >= 0) ? Vector3.right : Vector3.left;
            projectile.initialVelocity = Quaternion.Euler(0, 0, elevation) * direction * speed;
        }

        // call from AnimationEvent
        public void EventEmitDefault()
        {
            Emit(m_defaultSpeed, m_defaultElevation);
        }

        public void EventEmit(AnimationEvent ae)
        {
            Emit(ae.floatParameter, ae.intParameter);
        }
    }
}
