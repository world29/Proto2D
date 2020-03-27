using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    public class Parallax : MonoBehaviour
    {
        [SerializeField, Range(0, 1)]
        float m_parallaxEffect;

        [SerializeField]
        AnimationCurve m_alphaCurve;

        private Camera m_camera;
        private Vector3 m_basePos;
        private float m_distance;

        private void Awake()
        {
            if (m_camera == null)
            {
                m_camera = Camera.main;
            }
        }

        private void Start()
        {
            m_basePos = transform.position;

            // このオブジェクトの初期位置と、生成時点でのカメラ位置との差
            m_distance = Mathf.Abs(m_basePos.y - m_camera.transform.position.y);
        }

        private void LateUpdate()
        {
            float diff = m_camera.transform.position.y - m_basePos.y;

            transform.position = new Vector3(transform.position.x, m_basePos.y + diff * m_parallaxEffect, transform.position.z);

            {
                var renderer = GetComponent<SpriteRenderer>();

                // ratio = 0 ~ 1
                // 基準位置から離れるほど、0 に近づく
                var ratio = 1f - Mathf.Abs(diff) / m_distance;

                var clr = renderer.color;
                clr.a = m_alphaCurve.Evaluate(ratio);
                renderer.color = clr;
            }

        }
    }
}
