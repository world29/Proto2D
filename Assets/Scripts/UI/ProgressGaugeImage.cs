using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

namespace Proto2D
{
    public class ProgressGaugeImage : MonoBehaviour
    {
        [SerializeField]
        Image m_image;

        [SerializeField]
        Sprite[] m_spritePerPhase;

        private bool m_initialized = false;

        private System.IDisposable m_phaseHandle;

        private void LateUpdate()
        {
            if (!m_initialized)
            {
                var controller = GameObject.FindObjectOfType<GameController>();
                if (controller)
                {
                    controller.ObserveEveryValueChanged(x => x.Stage)
                        .Subscribe(stage => HandleStageChanged(stage));

                    m_initialized = true;
                }
            }
        }

        private void HandleStageChanged(StageController stage)
        {
            if (m_phaseHandle != null)
            {
                m_phaseHandle.Dispose();
            }

            m_phaseHandle = stage.ObserveEveryValueChanged(x => x.Phase)
                .Subscribe(phase => HandlePhaseChanged(phase));
        }

        private void HandlePhaseChanged(StagePhase phase)
        {
            if (m_spritePerPhase.Length > (int)phase)
            {
                m_image.sprite = m_spritePerPhase[(int)phase];
            }
        }
    }
}
