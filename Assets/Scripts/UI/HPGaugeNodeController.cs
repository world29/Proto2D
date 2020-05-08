using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Proto2D
{
    public class HPGaugeNodeController : MonoBehaviour
    {
        Animator m_animator;

        public void Awake()
        {
            m_animator = GetComponent<Animator>();
        }

        public void Heal()
        {
            m_animator.SetBool("damaged", false);
        }

        public void Damage()
        {
            m_animator.SetBool("damaged", true);
        }
    }
}
