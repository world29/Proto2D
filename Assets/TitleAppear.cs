using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Proto2D
{
    public class TitleAppear : MonoBehaviour
    {
        public CanvasGroup m_titleLogo;

        private TitleEffectState m_state = TitleEffectState.Ready;

        private void Start()
        {
            m_titleLogo.alpha = 0;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // プレイヤーがエリアに侵入したら、タイトルロゴをフェードインする
            if (collision.CompareTag("Player") && m_state == TitleEffectState.Ready)
            {
                Debug.Log("Title Appear");

                m_titleLogo.DOFade(1, 2).OnComplete(() => {
                    m_state = TitleEffectState.Done;
                });

                m_state = TitleEffectState.InProgress;
            }
        }

        enum TitleEffectState
        {
            Ready,
            InProgress,
            Done,
        }
    }
}
