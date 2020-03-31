using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    // ミサイルの挙動
    [RequireComponent(typeof(Projectile))]
    public class Missile : MonoBehaviour, IDamageSender
    {
        public Transform m_targetTransform;

        [Header("速度")]
        public float m_speed;

        [Header("この係数が大きいほどターゲットに追従しやすく、Rigidbody2D.AngularDrag が大きいほど追従しにくくなる")]
        public float m_ratio;

        [Header("アクティブ時間を過ぎるとターゲットに追従しなくなる")]
        public float m_activeTime = 5;

        [Header("ターゲット位置を更新する")]
        public bool m_updateTarget = false;

        private Vector3 m_targetPosition;

        void Start()
        {
            // ターゲットが設定されていなければプレイヤーを探してターゲットとする
            if (m_targetTransform == null)
            {
                var go = GameObject.FindGameObjectWithTag("Player");
                Debug.Assert(go);
                m_targetTransform = go.transform;
            }

            m_targetPosition = m_targetTransform.position;
        }

        private void Update()
        {
            // ターゲットの位置を更新する
            if (m_updateTarget)
            {
                m_targetPosition = m_targetTransform.position;
            }

            // アクティブ時間の計算
            m_activeTime -= Time.deltaTime;
        }

        private void FixedUpdate()
        {
            var rb = GetComponent<Rigidbody2D>();

            if (m_activeTime > 0)
            {
                var diff = m_targetPosition - transform.position;
                Debug.DrawRay(transform.position, diff);
                var q = Quaternion.FromToRotation(transform.right, diff);
                var rot = (q.eulerAngles.z < 180) ? q.eulerAngles.z : q.eulerAngles.z - 360;
                var torque = rot * m_ratio;
                rb.AddTorque(torque);
            }

            // 向いている方に進む
            rb.velocity = transform.right * m_speed;
        }

        public void OnApplyDamage(DamageType type, float damage, GameObject receiver)
        {
            GetComponent<Projectile>().m_OnHit.Invoke();

            Destroy(gameObject);
        }

        // デバッグ描画
        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                Gizmos.color = new Color(1, 0, 0, .3f);
                Gizmos.DrawSphere(m_targetPosition, 1);
            }
        }
    }
}
