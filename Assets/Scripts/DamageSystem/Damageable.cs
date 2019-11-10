using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Damageable : MonoBehaviour
    {
        [EnumFlags]
        public DamageTypeFlag m_damageTypeFlag = 
            DamageTypeFlag.Contact |
            DamageTypeFlag.Stomp |
            DamageTypeFlag.Attack |
            DamageTypeFlag.Projectile |
            DamageTypeFlag.FrailtyProjectile;

        public GameObject m_receiver;

        private void OnEnable()
        {
            GetComponent<BoxCollider2D>().enabled = true;
        }

        private void OnDisable()
        {
            GetComponent<BoxCollider2D>().enabled = false;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // nop
        }

        private void OnDrawGizmos()
        {
            BoxCollider2D collider = GetComponent<BoxCollider2D>();
            if (collider.enabled)
            {
                Gizmos.color = new Color(0, 1, 0, .2f);
                Gizmos.DrawCube(collider.bounds.center, collider.bounds.size);
            }
        }
    }
}
