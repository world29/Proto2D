using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Proto2D
{
    public class RandomEmitter : MonoBehaviour, IEmitter
    {
        [SerializeField, Header("抽選テーブル")]
        Entry[] m_table;

        [SerializeField, Header("生成位置。null ならこのコンポーネントがアタッチされているオブジェクトの位置。")]
        Transform m_locator;

        [SerializeField, Header("初速度")]
        float m_speed;

        [SerializeField, Header("± Angle Range の値で変動します")]
        float m_angleRange;

        [SerializeField, Header("± Speed Range の範囲で変動します")]
        float m_speedRange;

        // IEmitter
        public float Speed { get { return m_speed; } set { m_speed = value; } }

        // IEmitter
        public void Emit()
        {
            if (m_table.Length == 0) return;

            // 抽選
            var probabilities = m_table.Select(entry => entry.weight);
            var index = Randomize.ChooseWithProbabilities(probabilities.ToArray());

            Debug.Assert(index < m_table.Length);
            Debug.LogFormat("Object choosed {0}", m_table[index].description);

            if (m_table[index].entity)
            {
                var count = Random.Range(m_table[index].countRange.x, m_table[index].countRange.y + 1);
                for (int i = 0; i < count; i++)
                {
                    EmitEntity(m_table[index].entity);
                }
            }
        }

        void EmitEntity(Rigidbody2D entity)
        {
            Vector3 position = transform.position;
            Quaternion rotation = transform.rotation;

            if (m_locator)
            {
                position = m_locator.position;
                rotation = m_locator.rotation;
            }

            // 速度の変動を計算
            var tempSpeed = Speed;
            {
                Speed += Random.Range(-m_speedRange, m_speedRange);
            }

            // 角度の変動を計算
            var tempRot = rotation;
            {
                var angle = tempRot.eulerAngles;
                angle.z += Random.Range(-m_angleRange, m_angleRange);

                var q = Quaternion.Euler(angle.x, angle.y, angle.z);
                if (transform.lossyScale.x < 0)
                {
                    q = Quaternion.Euler(angle.x, angle.y, 180 - angle.z);
                }

                transform.rotation = q;
            }

            var rb = GameObject.Instantiate(entity, position, Quaternion.identity) as Rigidbody2D;

            // 初速を計算
            var initialVelocity = transform.rotation * Vector3.right * Speed;
            rb.velocity = initialVelocity;

            // デバッグ描画
            Debug.DrawLine(position, position + initialVelocity);

            // 速度と向きの変動を元に戻す
            transform.rotation = tempRot;
            Speed = tempSpeed;
        }

        [System.Serializable]
        public struct Entry
        {
            public string description; // デバッグ用の説明文
            public Rigidbody2D entity;
            public float weight; // 選択確率
            public Vector2Int countRange;
        }

        private void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if (m_angleRange > 0)
            {
                UnityEditor.Handles.color = new Color(0, 1, 0, .2f);

                var q_from = transform.rotation;
                if (transform.lossyScale.x < 0)
                {
                    var e = q_from.eulerAngles;
                    q_from = Quaternion.Euler(e.x, e.y, 180 - e.z);
                }
                q_from *= Quaternion.Euler(0, 0, -m_angleRange);

                Vector3 from = q_from * Vector3.right;

                UnityEditor.Handles.DrawSolidArc(
                    transform.position,
                    Vector3.forward,
                    from,
                    m_angleRange * 2,
                    Speed - m_speedRange);

                UnityEditor.Handles.color = new Color(0, 1, 0, .2f);

                UnityEditor.Handles.DrawSolidArc(
                    transform.position,
                    Vector3.forward,
                    from,
                    m_angleRange * 2,
                    Speed + m_speedRange);

                UnityEditor.Handles.color = Color.white;
            }
#endif
        }
    }
}
