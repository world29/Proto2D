using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    public class GameProgressController : MonoBehaviour
    {
        [Header("進捗度の最大値 (この値を超えると一段階アップ)")]
        public float m_maxProgressValue = 100;

        [Header("進捗の段階の上限")]
        public StagePhase m_stagePhaseLimit = StagePhase.Phase2;

        [Header("開始する段階")]
        public StagePhase m_startStagePhase = StagePhase.Phase1;

        [Header("10m登るたび増える進捗度")]
        public float m_progressPerTenMeter = 1;

        [HideInInspector]
        public NotificationObject<float> m_progress;
        [HideInInspector]
        public NotificationObject<StagePhase> m_stagePhase;

        private GameObject m_player;
        private float m_nextHightToProgress;

        private void Awake()
        {
            m_progress = new NotificationObject<float>(0);
            m_stagePhase = new NotificationObject<StagePhase>(m_startStagePhase);
        }

        void Start()
        {
            m_nextHightToProgress = 10;
        }

        void Update()
        {
        
        }

        private void LateUpdate()
        {
            ensurePlayer();

            if (m_player)
            {
                if (m_player.transform.position.y > m_nextHightToProgress)
                {
                    Debug.LogFormat("player got progress point :{0} meter", m_nextHightToProgress);

                    AddProgressValue(m_progressPerTenMeter);

                    m_nextHightToProgress += 10;
                }
            }
        }

        public void AddProgressValue(float value)
        {
            float nextValue = m_progress.Value + value;

            if (nextValue >= m_maxProgressValue)
            {
                if (m_stagePhase.Value < m_stagePhaseLimit)
                {
                    // 進捗度を一段階上げる
                    m_stagePhase.Value++;

                    nextValue -= m_maxProgressValue;
                }
                else
                {
                    // 価の変更があれば反映し、そうでなければ無視する
                    float temp = Mathf.Min(nextValue, m_maxProgressValue);
                    if (m_progress.Value < temp)
                    {
                        m_progress.Value = temp;
                    }
                }
            }

            m_progress.Value = nextValue;
        }

        private void ensurePlayer()
        {
            if (m_player == null)
            {
                m_player = GameObject.FindGameObjectWithTag("Player");
            }

            Debug.Assert(m_player != null);
        }
    }
}
