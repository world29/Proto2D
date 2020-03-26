using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    public class Parallax : MonoBehaviour
    {
        [SerializeField, Range(0, 1)]
        float m_parallaxEffect;

        private Camera m_camera;
        private Vector3 m_startPos;

        private void Awake()
        {
            if (m_camera == null)
            {
                m_camera = Camera.main;
            }
        }

        private void Start()
        {
            m_startPos = transform.position;
        }

        private void LateUpdate()
        {
            float dist = m_camera.transform.position.y * m_parallaxEffect;

            transform.position = new Vector3(transform.position.x, m_startPos.y + dist, transform.position.z);
        }
    }
}
