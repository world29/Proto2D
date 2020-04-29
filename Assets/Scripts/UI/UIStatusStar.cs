using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace Proto2D
{
    public class UIStatusStar : MonoBehaviour
    {
        [SerializeField]
        Image[] m_targetImages;

        private void Start()
        {
            if (m_targetImages.Length > 0)
            {
                GameController.Instance.RxStage
                    .Where(stage => stage != null)
                    .Subscribe(stage => {
                        SetEnabledImages(stage.IsCompleted);
                    });
            }
        }

        private void SetEnabledImages(bool enabled)
        {
            foreach(var image in m_targetImages)
            {
                image.enabled = enabled;
            }
        }
    }
}
