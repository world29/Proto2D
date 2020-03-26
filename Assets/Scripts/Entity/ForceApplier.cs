using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    public class ForceApplier : MonoBehaviour
    {
        [SerializeField]
        float m_force;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            foreach (var c in collision.contacts)
            {
                var rb = collision.gameObject.GetComponent<Rigidbody2D>();
                rb.AddForceAtPosition(c.normal * m_force, c.point, ForceMode2D.Impulse);
            }
        }
    }
}
