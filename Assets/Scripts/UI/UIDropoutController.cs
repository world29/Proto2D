using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using System;

namespace Proto2D
{
    public class UIDropoutController : MonoBehaviour
    {
        public Text m_counterText;
        public int m_timeLimit = 3;

        private PlayerController m_playerCache;
        private IDisposable m_countDownHandle;

        private void Awake()
        {
            Debug.Assert(m_counterText);
        }

        private void Start()
        {
            var countDownObservable = CreateCountDownObservable(m_timeLimit);

            // プレイヤーが画面外に出た/画面内に入った際に bool 値を発行する
            var playerFrameOutObservable = this.UpdateAsObservable()
                .Where(_ => !GameManager.Instance.isGameOver.Value)
                .Select(_ => IsPlayerFrameOut())
                .DistinctUntilChanged();

            playerFrameOutObservable
                .Subscribe(isFrameOut =>
                {
                    // カウントダウンのテキスト表示切替
                    m_counterText.enabled = isFrameOut;

                    if (isFrameOut)
                    {
                        // カウントダウン開始
                        m_countDownHandle = countDownObservable
                            .Subscribe(time =>
                            {
                                UpdateUI(time);
                                if (time == 0)
                                {
                                    // カウントゼロで強制ゲームオーバー
                                    Dropout();
                                }
                            }).AddTo(gameObject);
                    }
                    else
                    {
                        // カウントダウンをキャンセル
                        if (m_countDownHandle != null)
                        {
                            m_countDownHandle.Dispose();
                        }
                    }
                });
        }

        [ContextMenu("Dropout")]
        private void Dropout()
        {
            m_playerCache.ApplyDirectDamage(Mathf.Infinity);
        }

        private void UpdateUI(int counter)
        {
            m_counterText.text = string.Format("{0}", counter);
            m_counterText.rectTransform.localScale = Vector3.one * 1.5f;

            m_counterText.rectTransform
                .DOScale(Vector3.one, .5f);
        }

        private bool IsPlayerFrameOut()
        {
            if (m_playerCache == null)
            {
                GameObject go = GameObject.FindGameObjectWithTag("Player");
                if (go)
                {
                    m_playerCache = go.GetComponent<PlayerController>();
                }
            }
            Debug.Assert(m_playerCache != null);

            // プレイヤー画面外の判定
            Vector3 viewportPoint = Camera.main.WorldToViewportPoint(m_playerCache.transform.position);
            if (viewportPoint.y < -0.05f)
            {
                return true;
            }
            return false;
        }

        private IObservable<int> CreateCountDownObservable(int countTime)
        {
            return Observable
                .Timer(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1))
                .Select(x => (int)(countTime - x))
                .TakeWhile(x => x >= 0);
        }
    }
}

