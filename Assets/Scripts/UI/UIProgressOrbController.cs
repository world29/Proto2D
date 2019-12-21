using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

namespace Proto2D
{
    public class UIProgressOrbController : MonoBehaviour
    {
        [Header("移動パラメータ")]
        [Tooltip("進捗ゲージオブジェクトの RectTransform を指定する")]
        public RectTransform m_target;
        [Tooltip("移動アニメーションの時間")]
        public float m_duration = 1;
        [Tooltip("移動アニメーションのイージング")]
        public Ease m_easeType = Ease.Linear;

        [HideInInspector]
        public UnityAction OnCompleted { get; set; }

        private void Awake()
        {
            if (m_target == null)
            {
                //TODO: 名前によるオブジェクト検索は遅いので改善する
                var go = GameObject.Find("ProgressSlider");
                if (go)
                {
                    m_target = go.GetComponent<RectTransform>();
                }

            }
        }

        private void Start()
        {
            RectTransform rectTransform = transform as RectTransform;

            // 目標へ向かって移動する
            rectTransform.DOMove(m_target.position, m_duration)
                .SetEase(m_easeType)
                .OnComplete(() => {
                    // 完了時コールバックを呼び出す
                    OnCompleted();

                    // オブジェクトを削除する
                    Destroy(gameObject);
                });
        }
    }
}

