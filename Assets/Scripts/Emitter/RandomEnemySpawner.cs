using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Proto2D
{
    public class RandomEnemySpawner : MonoBehaviour, IEmitter
    {
        // 生成時の向きのタイプ
        public enum SpawnDirection { Front, Back, Random }

        [Header("モンスター抽選テーブル")]
        public EnemyEntry[] m_enemyTable;

        [Header("生成位置。null ならこのコンポーネントがアタッチされているオブジェクトの位置。")]
        public Transform m_locator;

        [Header("true ならスタート時に自動的にスポーンする (MonoBehaviour.Start)")]
        public bool spawnOnAwake = true;

        [Header("生成時の向き")]
        public SpawnDirection spawnDirection = SpawnDirection.Front;

        private void Start()
        {
            if (spawnOnAwake)
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
            Debug.LogFormat("Item choosed {0}", m_enemyTable[index].description);

            if (m_enemyTable[index].prefab)
            {
                Vector3 position = transform.position;
                Quaternion rotation = transform.rotation;

                if (m_locator)
                {
                    position = m_locator.position;
                    rotation = m_locator.rotation;
                }
                GameObject.Instantiate(m_enemyTable[index].prefab, position, Quaternion.identity);

                //TODO: 向きを計算
            }
        }

        [System.Serializable]
        public struct EnemyEntry
        {
            public string description; // デバッグ用の説明文
            public GameObject prefab;
            public float weight; // 選択確率
        }
    }
}
