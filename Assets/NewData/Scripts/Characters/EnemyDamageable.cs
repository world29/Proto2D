using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Assets.NewData.Scripts
{
    public class EnemyDamageable : MonoBehaviour, IDamageable
    {
        [SerializeField]
        private Health health;

        // IDamageable
        public bool TryDealDamage(float damageAmount)
        {
            health.ChangeHealth(-damageAmount);

            OnTakeDamage();

            return true;
        }

        private void OnTakeDamage()
        {
        }

        [ContextMenu("Kill")]
        public void Kill()
        {
            health.ChangeHealth(-Mathf.Infinity);
        }
    }
}
