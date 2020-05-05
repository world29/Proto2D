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

        private bool registered = false;

        private void Awake()
        {
            UpdateUI(0);
        }

        private void LateUpdate()
        {
            if (!registered)
            {
                GameObject go = GameObject.FindGameObjectWithTag("Player");
                if (go)
                {
                    var pc = go.GetComponent<PlayerController>();
                    pc.coinCount
                        .DistinctUntilChanged()
                        .Subscribe(count => UpdateUI(count));

                    registered = true;
                }
            }
        }

        private void UpdateUI(int coinCount)
        {
            m_counterText.text = string.Format("x{0}", coinCount);
            StartCoroutine(IncrementedEffect(m_scaleDuration));
        }

        IEnumerator IncrementedEffect(float duration)
        {
            float startTime = Time.time;
            float endTime = startTime + duration;

            while (Time.time < endTime)
            {
                float percentage = (Time.time - startTime) / duration;
                float value = m_scaleCurve.Evaluate(percentage);

                m_counterText.rectTransform.localScale = new Vector3(value, value, 1);

                yield return new WaitForEndOfFrame();
            }

            m_counterText.rectTransform.localScale = Vector3.one;
        }
    }
}

