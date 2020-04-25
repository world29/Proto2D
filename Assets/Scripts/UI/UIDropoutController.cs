using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Proto2D
{
    public class UIDropoutController : MonoBehaviour
    {
        public Text m_counterText;
        public int m_timeLimit = 3;

        private PlayerController m_player;
        private float m_timer;

        private void Awake()
        {
            Debug.Assert(m_counterText);
        }

        private void Start()
        {
            m_timer = m_timeLimit;
        }

        private void LateUpdate()
        {
            if (m_player == null)
            {
                GameObject go = GameObject.FindGameObjectWithTag("Player");
                if (go)
                {
                    m_player = go.GetComponent<PlayerController>();
                }
            }

            // プレイヤー画面外の判定
            bool isPlayerOutOfViewport = false;

            if (m_player)
            {
                Vector3 viewportPoint = Camera.main.WorldToViewportPoint(m_player.transform.position);
                if (viewportPoint.y < -0.05f)
                {
                    isPlayerOutOfViewport = true;
                }
            }

            // カウントダウン
            if (isPlayerOutOfViewport)
            {
                m_timer -= Time.deltaTime;
            }
            else
            {
                m_timer = m_timeLimit;
            }

            // カウント 0 でゲームオーバー
            if (Mathf.CeilToInt(m_timer) == 0)
            {
                m_player.ApplyDamage(Mathf.Infinity);
            }
            UpdateUI(Mathf.CeilToInt(m_timer));

            m_counterText.enabled = isPlayerOutOfViewport;
        }

        private void UpdateUI(int counter)
        {
            m_counterText.text = string.Format("{0}", counter);
        }
    }
}

