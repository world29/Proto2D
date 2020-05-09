using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Proto2D
{
    [ExecuteInEditMode()]
    public class RectTransformLockPosition : MonoBehaviour
    {
        [SerializeField]
        RectTransform m_lockTarget;

        private RectTransform m_rectTransform;

        private void Awake()
        {
            m_rectTransform = GetComponent<RectTransform>();
        }

        private void LateUpdate()
        {
#if true
            m_rectTransform.position = m_lockTarget.position;
#else
            var graphic = GetComponent<Graphic>();
            var canvas = graphic.canvas;

            var screenPoint = m_lockTarget.position;
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                m_rectTransform.position = screenPoint;
            }
            else if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                Vector2 localPosition = Vector2.zero;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(graphic.rectTransform, screenPoint, canvas.worldCamera, out localPosition);
                //m_rectTransform.anchoredPosition = localPosition;
            }
#endif
        }
    }
}
