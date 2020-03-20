using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Proto2D
{
    public class RandomEnemyEmitter : MonoBehaviour, IEmitter
    {
        [SerializeField, Header("モンスター抽選テーブル")]
        EnemyEntry[] m_enemyTable;

        [SerializeField, Header("生成位置。null ならこのコンポーネントがアタッチされているオブジェクトの位置。")]
        Transform m_locator;

        [SerializeField, Header("true ならスタート時に自動的に抽選する (MonoBehaviour.Start)")]
        bool m_emitOnAwake = true;

        private void Start()
        {
            if (m_emitOnAwake)
            {
                Emit();
            }
        }

        // IEmitter
        public void Emit()
        {
            if (m_enemyTable.Length == 0) return;

            // 抽選
            var probabilities = m_enemyTable.Select(entry => entry.weight);
            var index = Randomize.ChooseWithProbabilities(probabilities.ToArray());

            Debug.Assert(index < m_enemyTable.Length);
            Debug.LogFormat("Enemy choosed {0}", m_enemyTable[index].description);

            if (m_enemyTable[index].enemy)
            {
                Vector3 position = transform.position;

                if (m_locator)
                {
                    position = m_locator.position;
                }
                var go = GameObject.Instantiate(m_enemyTable[index].enemy, position, Quaternion.identity);
            }
        }

        [System.Serializable]
        public struct EnemyEntry
        {
            public string description; // デバッグ用の説明文
            public EnemyBehaviour enemy;
            public float weight; // 選択確率
        }
    }
}
