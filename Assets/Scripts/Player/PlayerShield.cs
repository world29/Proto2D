using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace Proto2D
{
    public class PlayerShield : MonoBehaviour
    {
        [SerializeField, Tooltip("同時に保持できるシールドの上限")]
        int m_shieldLimit;

        [SerializeField]
        IntReactiveProperty m_currentShield;

        public AudioClip m_consumeSE;

        AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }
        public IReadOnlyReactiveProperty<int> currentShield
        {
            get { return m_currentShield; }
        }

        public int shieldLimit { get { return m_shieldLimit; } }

        public void SetCurrentShield(int value)
        {
            m_currentShield.Value = value;
        }

        public void AddShield()
        {
            m_currentShield.Value = Mathf.Min(m_shieldLimit, m_currentShield.Value + 1);
        }

        public bool ConsumeShield()
        {
            if (m_currentShield.Value > 0)
            {
                if (m_consumeSE)
                {
                    audioSource.PlayOneShot(m_consumeSE);
                }

                m_currentShield.Value--;
                return true;
            }
            return false;
        }

        public void ResetShields()
        {
            m_currentShield.Value = 0;
        }
    }
}
