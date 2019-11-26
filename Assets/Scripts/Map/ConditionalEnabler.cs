using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    [System.Flags]
    public enum StagePhaseFlag
    {
        Phase1 = 0x1 << 0,
        Phase2 = 0x1 << 1,
        Phase3 = 0x1 << 2,
    };

    public class ConditionalEnabler : MonoBehaviour
    {
        [Tooltip("ステージ進捗度による出現の可否"), EnumFlags]
        public StagePhaseFlag m_stagePhaseFlags = StagePhaseFlag.Phase1 | StagePhaseFlag.Phase2 | StagePhaseFlag.Phase3;

        [Tooltip("出現確率"), Range(0, 100)]
        public int m_percentage = 100;

        void Start()
        {
            bool activated = false;

            StagePhaseFlag flags = (StagePhaseFlag)(0x1 << (int)GameController.Instance.Stage.Phase);
            if ((flags & m_stagePhaseFlags) > 0)
            {
                if (Random.Range(0, 100) < m_percentage)
                {
                    // 進捗度と出現確率の条件を満たした場合のみ
                    activated = true;
                }
            }

            gameObject.SetActive(activated);
        }
    }
}
