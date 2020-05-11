using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace Proto2D
{
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField, Tooltip("最大HP")]
        FloatReactiveProperty m_maxHealth;

        [SerializeField, Tooltip("最大HPの上限")]
        FloatReactiveProperty m_maxHealthLimit;

        [SerializeField]
        FloatReactiveProperty m_currentHealth;

        public IReadOnlyReactiveProperty<float> maxHealth
        {
            get { return m_maxHealth; }
        }

        public IReadOnlyReactiveProperty<float> currentHealth
        {
            get { return m_currentHealth; }
        }

        public void SetMaxHealth(float value)
        {
            m_maxHealth.Value = value;
        }

        public void SetCurrentHealth(float value)
        {
            m_currentHealth.Value = value;
        }

        public void ApplyDamage(float damageValue)
        {
            m_currentHealth.Value = Mathf.Max(m_currentHealth.Value - damageValue, 0f);
        }

        public void ApplyHeal(float healValue)
        {
            m_currentHealth.Value = Mathf.Min(m_currentHealth.Value + healValue, m_maxHealth.Value);
        }

        public bool IsDead()
        {
            return m_currentHealth.Value <= 0f;
        }
    }
}
