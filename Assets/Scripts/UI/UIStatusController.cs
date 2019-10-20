using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Proto2D
{
    public class UIStatusController : MonoBehaviour
    {
        public GameProgressController m_progressController;

        public Slider m_progressSlider;
        //public Image m_progressSliderBackground;
        public Image m_progressSliderFill;

        public Sprite[] m_progressSliderSprites;

        void Start()
        {
            m_progressSlider.maxValue = m_progressController.m_maxProgressValue;
            m_progressController.m_progress.OnChanged += OnProgressChanged;
            m_progressController.m_progressLevel.OnChanged += OnProgressLevelChanged;
        }

        void Update()
        {
        }

        void OnProgressChanged(float value)
        {
            m_progressSlider.value = value;
        }

        void OnProgressLevelChanged(int level)
        {
            // 進捗レベルに応じたゲージ用スプライトが設定されていることを保障
            Debug.Assert(m_progressSliderSprites.Length > level);

            //TODO: バックグラウンド
            m_progressSliderFill.sprite = m_progressSliderSprites[level];
        }

        void UpdateUI()
        {
        }
    }
}

