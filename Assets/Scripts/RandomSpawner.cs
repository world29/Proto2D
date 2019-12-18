using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Proto2D
{
    public class RandomSpawner : MonoBehaviour
    {
        public SpawnEntry[] m_entries;

        private const float kSpawnSpeed = 10;
        private const float kSpawnAngleRange = 20;

        [ContextMenu("Spawn")]
        public void Spawn()
        {
            // 抽選
            float rand = Random.value;

            float totalWeight = m_entries.Aggregate(0, (acc, cur) => acc + cur.weight);

            float accumulated = 0;
            for (int i = 0; i < m_entries.Length; i++)
            {
                accumulated += m_entries[i].weight;
                float bounds = accumulated / totalWeight;
                if (rand < bounds)
                {
                    if (m_entries[i].prefab != null)
                    {
                        SpawnPrefab(m_entries[i].prefab);
                    }
                    break;
                }
            }
        }

        private void SpawnPrefab(GameObject prefab)
        {
            GameObject go = GameObject.Instantiate(prefab, transform.position, Quaternion.identity);

            DynamicObject dynobj = go.GetComponent<DynamicObject>();
            if (dynobj)
            {
                float deg = Random.value * kSpawnAngleRange - (kSpawnAngleRange / 2);

                Vector2 itemVelocity = Quaternion.Euler(0, 0, deg) * (Vector2.up * kSpawnSpeed);
                dynobj.Initialize(itemVelocity);
            }
        }

        [System.Serializable]
        public struct SpawnEntry
        {
            [Tooltip("スポーンしやすさを表す重み値。(スポーン確率 = エントリの重み値 / 全エントリの重み合計値)")]
            public int weight;
            [Tooltip("生成されるプレハブ。None の場合は何も生成しないエントリとして動作します。")]
            public GameObject prefab;
        }
    }
}
