using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Proto2D
{
    public class UIStatusController : MonoBehaviour
    {
        [Header("Progress")]
        public Slider m_progressSlider;
        public Image m_progressSliderBackground;
        public Image m_progressSliderFill;

        public Sprite[] m_progressSliderBackgroundSprites;
        public Sprite[] m_progressSliderFillSprites;

        [Header("Health")]
        public Slider m_healthSlider;

        private bool m_healthCallbackRegistered = false;
        private StageController m_stage;

        void Start()
        {
        }

        void LateUpdate()
        {
            if (!m_healthCallbackRegistered)
            {
                GameObject go = GameObject.FindGameObjectWithTag("Player");
                if (go)
                {
                    PlayerController pc = go.GetComponent<PlayerController>();

                    m_healthSlider.maxValue = pc.initialHealth;
                    m_healthSlider.value = pc.initialHealth;
                    pc.health.OnChanged += OnPlayerHealthChanged;

                    m_healthCallbackRegistered = true;
                }
            }
        }

        public void ResetStage(StageController prevStage, StageController nextStage)
        {
            if (prevStage)
            {
                prevStage.m_progress.OnChanged -= OnProgressChanged;
                prevStage.m_phase.OnChanged -= OnPhaseChanged;
            }

            m_progressSlider.maxValue = nextStage.m_progressPerPhase;

            nextStage.m_progress.OnChanged += OnProgressChanged;
            nextStage.m_phase.OnChanged += OnPhaseChanged;

            m_stage = nextStage;

            // 明示的に呼び出してリセットする
            OnProgressChanged(nextStage.Progress);
            OnPhaseChanged(nextStage.Phase);
        }

        void OnProgressChanged(float value)
        {
            m_progressSlider.value = value % m_stage.m_progressPerPhase;
        }

        void OnPhaseChanged(StagePhase level)
        {
            // 進捗レベルに応じたゲージ用スプライトが設定されていることを保障
            Debug.Assert(m_progressSliderFillSprites.Length > (int)level);

            m_progressSliderBackground.sprite = m_progressSliderBackgroundSprites[(int)level];
            m_progressSliderFill.sprite = m_progressSliderFillSprites[(int)level];
        }

        void OnPlayerHealthChanged(float healthValue)
        {
            m_healthSlider.value = healthValue;
        }

        void UpdateUI()
        {
        }
    }
}

