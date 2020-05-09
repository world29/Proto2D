using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Proto2D
{
    [ExecuteInEditMode()]
    public class SlidableMask : MonoBehaviour
    {
        [SerializeField, Range(0f, 1f)]
        float m_value;

        public float sliderValue { get { return m_value; } set { m_value = value; } }

        [SerializeField]
        RectTransform m_lockTarget;

        RectTransform m_rectTransform;
        Vector3 m_farBottom;
        Vector3 m_farTop;

        private void Awake()
        {
            m_rectTransform = GetComponent<RectTransform>();

            m_farBottom = m_lockTarget.localPosition - new Vector3(0f, m_rectTransform.rect.height);
            m_farTop = m_lockTarget.localPosition;
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
            m_rectTransform.localPosition = Vector2.Lerp(m_farBottom, m_farTop, value);
        }
    }
}
