using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

namespace Proto2D
{
    public class ProgressGaugeController : MonoBehaviour
    {
        [SerializeField]
        SlidableMask m_mask;

        [SerializeField]
        Animator m_animator;

#if UNITY_EDITOR
        [SerializeField]
        private float m_maxProgress = 1;
        [SerializeField]
        private float m_currentProgress;
        [SerializeField]
        private StagePhase m_currentPhase;
#else
        private float m_maxProgress = 1;
        private float m_currentProgress;
#endif
        private bool m_initialized = false;

        private System.IDisposable m_progressHandle;
        private System.IDisposable m_phaseHandle;

        private void Start()
        {
        }

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
            m_maxProgress = stage.ProgressPerPhase;

            if (m_phaseHandle != null)
            {
                m_phaseHandle.Dispose();
            }

            m_phaseHandle = stage.ObserveEveryValueChanged(x => x.Phase)
                .Subscribe(phase => HandlePhaseChanged(phase));

            if (m_progressHandle != null)
            {
                m_progressHandle.Dispose();
            }

            m_progressHandle = stage.ObserveEveryValueChanged(x => x.Progress)
                .Subscribe(progress => HandleProgressChanged(progress));
        }

        private void HandlePhaseChanged(StagePhase phase)
        {
            m_currentPhase = phase;
        }

        private void HandleProgressChanged(float progress)
        {
            m_currentProgress = progress - (m_maxProgress * (int)m_currentPhase);
            m_mask.sliderValue = m_currentProgress / m_maxProgress;

            m_animator.SetTrigger("point_up");
        }
    }
}
