using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Proto2D
{
    public class UILoupeController : MonoBehaviour
    {
        public CanvasGroup m_target;

        private RectTransform m_rectTransform;
        private PlayerController m_player;

        private void Awake()
        {
            m_rectTransform = GetComponent<RectTransform>();
        }
        void Start()
        {
        }

        void LateUpdate()
        {
            if (m_player == null)
            {
                GameObject go = GameObject.FindGameObjectWithTag("Player");
                if (go)
                {
                    m_player = go.GetComponent<PlayerController>();
                }
            }

            // プレイヤーがビューポートの外にいる場合のみルーペを表示する
            bool isPlayerOutOfViewport = false;

            if (m_player)
            {
                Vector3 viewportPoint = Camera.main.WorldToViewportPoint(m_player.transform.position);
                if (viewportPoint.y < -0.05f)
                {
                    isPlayerOutOfViewport = true;

                    Vector3 pos = m_rectTransform.position;
                    pos.x = m_player.transform.position.x;
                    m_rectTransform.position = pos;
                }
            }

            if (isPlayerOutOfViewport)
            {
                m_target.DOFade(1, .5f);
            }
            else
            {
                m_target.DOFade(0, .5f);
            }
        }

        void UpdateUI()
        {
        }
    }
}

