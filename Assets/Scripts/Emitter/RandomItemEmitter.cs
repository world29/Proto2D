using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Proto2D
{
    public class RandomItemEmitter : MonoBehaviour, IProjectileEmitter
    {
        [Header("アイテム抽選テーブル")]
        public ItemEntry[] itemTable;

        [Header("生成位置。null ならこのコンポーネントがアタッチされているオブジェクトの位置。")]
        public Transform locator;

        [Header("初速度")]
        public float speed;

        // IProjectileEmitter
        public void Emit()
        {
            if (itemTable.Length == 0) return;

            // 抽選
            var probabilities = itemTable.Select(entry => entry.weight);
            var index = Randomize.ChooseWithProbabilities(probabilities.ToArray());

            Debug.Assert(index < itemTable.Length);
            Debug.LogFormat("Item choosed {0}", itemTable[index].description);

            if (itemTable[index].item)
            {
                Vector3 position = transform.position;
                Quaternion rotation = transform.rotation;

                if (locator)
                {
                    position = locator.position;
                    rotation = locator.rotation;
                }

                var rb = GameObject.Instantiate(itemTable[index].item, position, Quaternion.identity) as Rigidbody2D;

                Vector3 direction = (transform.lossyScale.x >= 0) ? Vector3.right : Vector3.left;
                Vector3 initialVelocity = rotation * direction * speed;

                rb.velocity = initialVelocity;

                Debug.DrawLine(position, position + initialVelocity);
            }
        }

        [System.Serializable]
        public struct ItemEntry
        {
            public string description; // デバッグ用の説明文
            public Rigidbody2D item;
            public float weight; // 選択確率
        }
    }
}
