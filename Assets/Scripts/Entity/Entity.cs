using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    public class Entity : MonoBehaviour, IDamageSender, IDamageReceiver
    {
        public void OnApplyDamage(DamageType type, float damage, GameObject receiver)
        {
            //Debug.LogFormat("{0}: damage applied to {1}", gameObject.name, receiver.name);
        }

        public void OnReceiveDamage(DamageType type, float damage, GameObject sender)
        {
            Debug.LogFormat("{0}: damage received from {1}", gameObject.name, sender.name);
        }
    }
}
