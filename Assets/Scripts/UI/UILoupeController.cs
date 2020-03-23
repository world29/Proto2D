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

        private void Awake()
        {
            m_rectTransform = GetComponent<RectTransform>();
        }
        void Start()
        {
        }

        void LateUpdate()
        {
            GameObject go = GameObject.FindGameObjectWithTag("Player");
            if (go == null)
            {
                return;
            }

            PlayerController player = go.GetComponent<PlayerController>();

            // プレイヤーがビューポートの外にいる場合のみルーペを表示する
            bool isPlayerOutOfViewport = false;

            Vector3 viewportPoint = Camera.main.WorldToViewportPoint(player.transform.position);
            if (viewportPoint.y < -0.05f)
            {
                isPlayerOutOfViewport = true;

                Vector3 pos = m_rectTransform.position;
                pos.x = player.transform.position.x;
                m_rectTransform.position = pos;
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

