using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.NewData.Scripts
{
    [RequireComponent(typeof(Health))]
    public class Damageable : MonoBehaviour
    {
        private Health _health;

        private void Awake()
        {
            _health = GetComponent<Health>();
        }

        public void DealDamage(float damageAmount)
        {
            _health.ChangeHealth(-damageAmount);

            Debug.Log($"Damage: {damageAmount}");
        }
    }
}
