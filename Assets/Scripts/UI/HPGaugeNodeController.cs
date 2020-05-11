using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Proto2D
{
    public class HPGaugeNodeController : MonoBehaviour
    {
        Animator m_animator;

        private static readonly int hashDamaged = Animator.StringToHash("HPGaugeNode_Damaged");
        private static readonly int hashHealed = Animator.StringToHash("HPGaugeNode_Fill");


        public void Awake()
        {
            m_animator = GetComponent<Animator>();
        }

        public IEnumerator HealAnimationFlow()
        {
            m_animator.Play(hashHealed);
            yield return null; // ステートの反映に 1 フレーム必要
            yield return new WaitForAnimation(m_animator, 0);
        }

        public IEnumerator DamageAnimationFlow()
        {
            m_animator.Play(hashDamaged);
            yield return null; // ステートの反映に 1 フレーム必要
            yield return new WaitForAnimation(m_animator, 0);
        }

        public void Heal()
        {
            m_animator.Play(hashHealed);
        }

        public void Damage()
        {
            m_animator.Play(hashDamaged);
        }
    }
}
