﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    public class Laser : MonoBehaviour
    {
        public Transform m_target;

        [Header("命中までの時間")]
        public float m_period;

        public GameObject m_hitEffectPrefab;

        [HideInInspector]
        public Vector3 initialVelocity { set { m_velocity = value; } }

        private Vector3 m_velocity;
        private Vector3 m_position;

        void Start()
        {
            if (m_target == null)
            {
                var go = GameObject.FindGameObjectWithTag("Player");
                if (go)
                {
                   m_target = go.GetComponent<Transform>();
                }
            }

            m_position = transform.position;
        }

        void Update()
        {
            var acceleration = Vector3.zero;

            var diff = m_target.position - m_position;
            acceleration += (diff - m_velocity * m_period) * 2f / (m_period * m_period);

            m_period -= Time.deltaTime;
            if (m_period < 0)
            {
                if (m_hitEffectPrefab)
                {
                    GameObject.Instantiate(m_hitEffectPrefab, transform.position, Quaternion.identity);
                }
                GameObject.Destroy(gameObject);
                return;
            }

            m_velocity += acceleration * Time.deltaTime;
            m_position += m_velocity * Time.deltaTime;
            transform.position = m_position;
        }
    }
}