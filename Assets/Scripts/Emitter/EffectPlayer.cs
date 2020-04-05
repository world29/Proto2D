using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Proto2D
{
    public class EffectPlayer : MonoBehaviour, IEmitter
    {
        [Header("エフェクト")]
        public GameObject m_effect;

        [Header("生成位置。null ならこのコンポーネントがアタッチされているオブジェクトの位置。")]
        public Transform m_locator;

        // IEmitter
        public float Speed { get; set; }

        // IEmitter
        public void Emit()
        {
            Vector3 position = transform.position;

            if (m_locator)
            {
                position = m_locator.position;
            }

            GameObject.Instantiate(m_effect, position, Quaternion.identity);
        }
    }
}
