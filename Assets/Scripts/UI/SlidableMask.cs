using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Proto2D
{
    public class SlidableMask : MonoBehaviour
    {
        [SerializeField, Range(0f, 1f)]
        float m_value;

        RectTransform m_rectTransform;
        Vector3 m_farBottom;
        Vector3 m_farTop;

        private void Awake()
        {
            m_rectTransform = GetComponent<RectTransform>();

            m_farBottom = m_rectTransform.position - new Vector3(0f, m_rectTransform.rect.height);
            m_farTop = m_rectTransform.position;
        }

        private void Start()
        {
            HandleSliderChanged(m_value);
        }

        private void LateUpdate()
        {
            HandleSliderChanged(m_value);
        }

        private void HandleSliderChanged(float value)
        {
            m_rectTransform.position = Vector2.Lerp(m_farBottom, m_farTop, value);
        }
    }
}
