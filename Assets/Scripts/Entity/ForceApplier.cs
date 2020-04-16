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

        private void OnCollisionEnter2D(Collision2D collision)
        {
            var rb = collision.gameObject.GetComponent<Rigidbody2D>();
            Debug.Assert(rb);

            // 物理制御オブジェクトのみを対象とする
            if (rb.isKinematic) return;

            foreach (var c in collision.contacts)
            {
                var entity = collision.gameObject.GetComponent<PhysicsEntity>();

                if (entity)
                {
                    entity.EnableMoving();
                }

                if (entity)// && entity.isContactObstacle)
                {
                    Vector2 dir = c.normal + Vector2.up;
                    rb.AddForceAtPosition(dir.normalized * m_force, collision.gameObject.transform.position, m_forceMode);
                }
                else
                {
                    rb.AddForceAtPosition(c.normal * m_force, collision.gameObject.transform.position, m_forceMode);
                }
            }
        }
    }
}
