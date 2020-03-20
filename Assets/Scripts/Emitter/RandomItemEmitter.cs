using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Proto2D
{
    public class RandomItemEmitter : MonoBehaviour, IEmitter
    {
        [Header("アイテム抽選テーブル")]
        public ItemEntry[] m_itemTable;

        [Header("生成位置。null ならこのコンポーネントがアタッチされているオブジェクトの位置。")]
        public Transform m_locator;

        [Header("初速度")]
        public float m_speed;

        // IEmitter
        public void Emit()
        {
            if (m_itemTable.Length == 0) return;

            // 抽選
            var probabilities = m_itemTable.Select(entry => entry.weight);
            var index = Randomize.ChooseWithProbabilities(probabilities.ToArray());

            Debug.Assert(index < m_itemTable.Length);
            Debug.LogFormat("Item choosed {0}", m_itemTable[index].description);

            if (m_itemTable[index].item)
            {
                Vector3 position = transform.position;
                Quaternion rotation = transform.rotation;

                if (m_locator)
                {
                    position = m_locator.position;
                    rotation = m_locator.rotation;
                }

                var rb = GameObject.Instantiate(m_itemTable[index].item, position, Quaternion.identity) as Pickup;

                // 初速を計算
                Vector3 direction = (transform.lossyScale.x >= 0) ? Vector3.right : Vector3.left;
                Vector3 initialVelocity = rotation * direction * m_speed;

                rb.rigidbody.velocity = initialVelocity;

                Debug.DrawLine(position, position + initialVelocity);
            }
        }

        [System.Serializable]
        public struct ItemEntry
        {
            public string description; // デバッグ用の説明文
            public Pickup item;
            public float weight; // 選択確率
        }
    }
}
