using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    [ExecuteInEditMode()]
    public class RectTransformLockPosition : MonoBehaviour
    {
        [SerializeField]
        RectTransform m_lockTarget;

        RectTransform m_rectTransform;

        private void Awake()
        {
            m_rectTransform = GetComponent<RectTransform>();
        }

        private void LateUpdate()
        {
            m_rectTransform.position = m_lockTarget.position;
        }
    }
}
