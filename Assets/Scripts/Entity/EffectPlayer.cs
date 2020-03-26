using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Proto2D
{
    public class EffectPlayer : MonoBehaviour
    {
        [Header("生成位置。null ならこのコンポーネントがアタッチされているオブジェクトの位置。")]
        public Transform m_locator;

        private void Awake()
        {
            if (m_locator == null)
            {
                m_locator = transform;
            }
        }

        public void PlayEffect(GameObject effect)
        {
            Debug.Assert(m_locator != null);

            GameObject.Instantiate(effect, m_locator.position, Quaternion.identity);
        }
    }
}
