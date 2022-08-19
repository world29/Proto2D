using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.NewData.Scripts
{
    public class CollisionDamage : MonoBehaviour
    {
        [SerializeField]
        private float damageAmount = 1f;

        public void OnTriggerEnter2D(Collider2D other)
        {
            GameObject hitObject = other.gameObject;
            TryDamageObject(hitObject);
        }

        private void TryDamageObject(GameObject objectToDamage)
        {
            if (objectToDamage.TryGetComponent(out Damageable damageable))
            {
                damageable.DealDamage(damageAmount);
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            // ダメージ判定の矩形描画
            if (TryGetComponent(out BoxCollider2D boxCollider2d))
            {
                Gizmos.color = new Color(1, 0, 0, .2f);
                Gizmos.DrawCube(boxCollider2d.bounds.center, boxCollider2d.bounds.size);
            }
        }
#endif
    }
}
