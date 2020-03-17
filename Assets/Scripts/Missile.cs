using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    public class Missile : MonoBehaviour
    {
        public LayerMask m_collisionMask;

        public Transform m_targetTransform;

        [Header("速度")]
        public float m_speed;

        [Header("この係数が大きいほどターゲットに追従しやすく、Rigidbody2D.AngularDrag が大きいほど追従しにくくなる")]
        public float m_ratio;

        [Header("寿命が終わるとターゲットに追従しなくなり、画面外にでると消える")]
        public float m_lifetime = 5;

        [Header("ターゲット位置を更新する")]
        public bool m_updateTarget = false;

        public GameObject m_hitEffectPrefab;

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

            // ビューポートの外に出たら消える
            {
                var camera = Camera.main;
                Vector3 vp = camera.WorldToViewportPoint(transform.position);

                const float margin = .5f;
                if (vp.x < -margin || vp.x > 1 + margin || vp.y < -margin || vp.y > 1 + margin)
                {
                    Destroy(gameObject);
                }
            }

            // 寿命の計算
            m_lifetime -= Time.deltaTime;
        }

        private void FixedUpdate()
        {
            var rb = GetComponent<Rigidbody2D>();

            if (m_lifetime > 0)
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

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                GameObject.Instantiate(m_hitEffectPrefab, transform.position, transform.rotation);

                GameObject.Destroy(gameObject);
            }
            else if (m_collisionMask == (m_collisionMask | (1 << collision.gameObject.layer)))
            {
                GameObject.Destroy(gameObject);
            }
        }

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
