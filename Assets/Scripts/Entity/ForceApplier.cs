using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    public class ForceApplier : MonoBehaviour
    {
        [SerializeField]
        ForceMode2D m_forceMode = ForceMode2D.Force;

        [SerializeField]
        float m_force;

        private void OnTriggerEnter2D(Collider2D collider)
        {
            collider.gameObject.GetComponent<PhysicsEntity>()?.Shoot();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            var rb = collision.gameObject.GetComponent<Rigidbody2D>();
            Debug.Assert(rb);

            // 物理制御オブジェクトのみを対象とする
            if (rb.isKinematic) return;

            collision.gameObject.GetComponent<PhysicsEntity>()?.Shoot();
        }
    }
}
