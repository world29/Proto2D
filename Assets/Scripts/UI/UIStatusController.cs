using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Proto2D
{
    public class UIStatusController : MonoBehaviour
    {
        public Slider m_progressSlider;
        //public Image m_progressSliderBackground;
        public Image m_progressSliderFill;

        public Sprite[] m_progressSliderSprites;

        private int m_progressLevel = 0;

        void Start()
        {
            Debug.Assert(GameController.Instance.m_progress != null);
            GameController.Instance.m_progress.OnChanged += OnProgressChanged;
        }

        void Update()
        {
        }

        void OnProgressChanged(float value)
        {
            int level = Mathf.FloorToInt(value / m_progressSlider.maxValue);

            bool canProgressGaugeUpdate = true;
            if (m_progressLevel < level)
            {
                if (m_progressSliderSprites.Length > level)
                {
                    // 進捗ゲージのレベルを一段階上げる
                    m_progressLevel = level;
                    m_progressSliderFill.sprite = m_progressSliderSprites[m_progressLevel];
                }
                else
                {
                    // レベルに応じたスプライトがない場合は、進捗度をゲージに反映しない
                    canProgressGaugeUpdate = false;
                }
            }

            if (canProgressGaugeUpdate)
            {
                float wrappedValue = value % m_progressSlider.maxValue;
                m_progressSlider.value = wrappedValue;
            }
        }

        void UpdateUI()
        {
        }
    }
}

