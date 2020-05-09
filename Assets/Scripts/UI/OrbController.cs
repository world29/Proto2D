using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Proto2D
{
    public class OrbController : MonoBehaviour
    {
        [SerializeField]
        float m_period;

        [HideInInspector]
        public Vector3 initialVelocity { set { m_velocity = value; } }

        private Vector3 m_velocity;
        private Vector3 m_position;
        private RectTransform m_target;
        private RectTransform m_rectTransform;

        private void Awake()
        {
            m_rectTransform = GetComponent<RectTransform>();
        }

        void Start()
        {
            if (m_target == null)
            {
                var go = GameObject.FindGameObjectWithTag("ProgressGauge");
                if (go)
                {
                    m_target = go.GetComponent<RectTransform>();
                }
            }

            m_position = m_rectTransform.position;
        }

        private void LateUpdate()
        {
            var acceleration = Vector3.zero;

            var diff = m_target.position - m_position;
            acceleration += (diff - m_velocity * m_period) * 2f / (m_period * m_period);

            m_period -= Time.deltaTime;
            if (m_period < 0)
            {
                if (GameController.Instance)
                {
                    GameController.Instance.AddProgressValue(1);
                }

                Destroy(gameObject);
                return;
            }

            m_velocity += acceleration * Time.deltaTime;
            m_position += m_velocity * Time.deltaTime;
            m_rectTransform.position = m_position;
        }

    }
}
