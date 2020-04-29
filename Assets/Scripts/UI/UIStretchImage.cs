using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Proto2D
{
    [ExecuteInEditMode]
    public class UIStretchImage : MonoBehaviour
    {
        [SerializeField]
        RectTransform m_sourceRect;

        [SerializeField]
        Image m_referenceImage;

        [SerializeField]
        RectTransform m_targetRect;

        private void Update()
        {
            if (m_sourceRect && m_referenceImage && m_targetRect)
            {
                var srcSize = m_sourceRect.sizeDelta;
                var refBounds = m_referenceImage.sprite.bounds;

                var scale = Mathf.Min(srcSize.x / refBounds.size.x, srcSize.y / refBounds.size.y);

                m_targetRect.sizeDelta = refBounds.size * scale;
            }
        }
    }
}
