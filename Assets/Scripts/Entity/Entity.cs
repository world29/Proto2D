using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Proto2D
{
    public class Entity : MonoBehaviour, IDamageSender, IDamageReceiver
    {
        [SerializeField]
        UnityEvent m_OnApplyDamage;

        [SerializeField]
        UnityEvent m_OnReceiveDamage;

        public void OnApplyDamage(DamageType type, float damage, GameObject receiver)
        {
            Debug.LogFormat("{0}: damage applied to {1}", gameObject.name, receiver.name);

            m_OnApplyDamage.Invoke();
        }

        public void OnReceiveDamage(DamageType type, float damage, GameObject sender)
        {
            Debug.LogFormat("{0}: damage received from {1}", gameObject.name, sender.name);

            m_OnReceiveDamage.Invoke();
        }

        public void KillImmediately()
        {
            Destroy(gameObject);
        }
    }
}
