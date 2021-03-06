﻿using System.Collections;
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

        [Header("初速度")]
        public float m_speed;

        [SerializeField, Header("true ならスタート時に自動的に抽選する (MonoBehaviour.Start)")]
        bool m_emitOnAwake = true;

        // IEmitter
        public float Speed { get { return m_speed; } set { m_speed = value; } }

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
                Quaternion rotation = transform.rotation;

                if (m_locator)
                {
                    position = m_locator.position;
                    rotation = m_locator.rotation;
                }

                var enemy = GameObject.Instantiate(m_enemyTable[index].enemy, position, Quaternion.identity) as EnemyBehaviour;

                // 初速を計算
                Vector3 direction = (transform.lossyScale.x >= 0) ? Vector3.right : Vector3.left;
                Vector3 initialVelocity = rotation * direction * m_speed;
                enemy.velocity = initialVelocity;
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
