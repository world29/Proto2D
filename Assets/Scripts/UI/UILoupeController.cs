using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Proto2D
{
    public class UILoupeController : MonoBehaviour
    {
        public Image m_loupeImage;

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

                    // キャンバス座標 (= スクリーン座標)
                    Vector3 pos = m_rectTransform.position;
                    pos.x = Screen.width * viewportPoint.x;
                    m_rectTransform.position = pos;
                }
            }

            if (isPlayerOutOfViewport)
            {
                m_loupeImage.color = Color.white;
            }
            else
            {
                m_loupeImage.color = new Color(1, 1, 1, 0);
            }
        }

        void UpdateUI()
        {
        }
    }
}

