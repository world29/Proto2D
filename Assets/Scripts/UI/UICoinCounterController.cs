using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;

namespace Proto2D
{
    public class UICoinCounterController : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI m_counterText;

        [Header("カウントアップ時のスケールアニメーション")]
        [Tooltip("アニメーション時間")]
        public float m_scaleDuration = .2f;
        [SerializeField, Tooltip("スケール値のカーブ")]
        public AnimationCurve m_scaleCurve;

        private void Start()
        {
            var playerWallet = FindObjectOfType<PlayerWallet>();
            playerWallet.coins
                .Subscribe(coins => UpdateUI(coins))
                .AddTo(gameObject);
        }

        private void UpdateUI(int coins)
        {
            m_counterText.text = string.Format("x{0}", coins);
            StartCoroutine(IncrementedEffect(m_scaleDuration));
        }

        IEnumerator IncrementedEffect(float duration)
        {
            float startTime = Time.unscaledTime;
            float endTime = startTime + duration;

            while (Time.unscaledTime < endTime)
            {
                float percentage = (Time.unscaledTime - startTime) / duration;
                float value = m_scaleCurve.Evaluate(percentage);

                m_counterText.rectTransform.localScale = new Vector3(value, value, 1);

                yield return new WaitForEndOfFrame();
            }

            m_counterText.rectTransform.localScale = Vector3.one;
        }
    }
}

