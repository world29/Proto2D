﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace Proto2D
{
    public class UICoinCounterController : MonoBehaviour
    {
        public Text m_counterText;

        [Header("カウントアップ時のスケールアニメーション")]
        [Tooltip("アニメーション時間")]
        public float m_scaleDuration = .2f;
        [SerializeField, Tooltip("スケール値のカーブ")]
        public AnimationCurve m_scaleCurve;

        private bool registered = false;

        private void Awake()
        {
            Debug.Assert(m_counterText);

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
                        .Subscribe(count => UpdateUI(count));

                    registered = true;
                }
            }
        }

        private void UpdateUI(int coinCount)
        {
            m_counterText.text = string.Format("x {0}", coinCount);
            StartCoroutine(IncrementedEffect(m_scaleDuration));
        }

        IEnumerator IncrementedEffect(float duration)
        {
            float startTime = Time.timeSinceLevelLoad;
            float endTime = startTime + duration;

            while (Time.timeSinceLevelLoad < endTime)
            {
                float percentage = (Time.timeSinceLevelLoad - startTime) / duration;
                float value = m_scaleCurve.Evaluate(percentage);

                m_counterText.rectTransform.localScale = new Vector3(value, value, 1);

                yield return new WaitForEndOfFrame();
            }

            m_counterText.rectTransform.localScale = Vector3.one;
        }
    }
}

