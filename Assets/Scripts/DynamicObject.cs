using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Proto2D
{
    // 自由落下するオブジェクト
    [RequireComponent(typeof(Rigidbody2D), typeof(Controller2D))]
    public class DynamicObject : MonoBehaviour
    {
        [Tooltip("重力加速度")]
        public float m_gravity = 20;

        [Tooltip("落下の効果音")]
        public AudioClip m_dropSound;

        private Controller2D m_controller;
        private Vector2 m_velocity;

        private void Awake()
        {
            m_controller = GetComponent<Controller2D>();
            m_velocity = Vector2.zero;
        }

        private void Update()
        {
            if (m_controller.collisions.below)
            {
                if (Mathf.Abs(m_velocity.y) > 3f)
                {
                    OnDrop();
                    m_velocity.y *= -0.8f;
                    m_velocity.x *= 0.8f;
                }
                else
                {
                    m_velocity.y = 0;
                }
            }
            else
            {
                m_velocity.y -= m_gravity * Time.deltaTime;
            }

            m_controller.Move(m_velocity * Time.deltaTime);
        }

        public void Initialize(Vector2 initialVelocity)
        {
            m_velocity = initialVelocity;
        }

        void OnDrop()
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource)
            {
                audioSource.PlayOneShot(m_dropSound);
            }
        }

        private void OnDrawGizmos()
        {
            if (m_controller && m_controller.collider)
            {
                Gizmos.color = new Color(.5f, .5f, .5f, .3f);
                Gizmos.DrawCube(m_controller.collider.bounds.center, m_controller.collider.bounds.size);
            }
        }
    }
}
