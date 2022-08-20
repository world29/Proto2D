using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.NewData.Scripts
{
    public class Damageable : MonoBehaviour
    {
        public UnityEngine.Events.UnityAction<float> OnTakeDamage;

        public void DealDamage(float damageAmount)
        {
            OnTakeDamage?.Invoke(damageAmount);
        }
    }
}
