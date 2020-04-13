using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Rotator : MonoBehaviour
    {
        [SerializeField, Header("速度に応じて回転するオブジェクト")]
        GameObject m_targetObject;

        [SerializeField]
        float m_offsetRotate = 0f;

        Rigidbody2D m_rigidbody;

        private void Start()
        {
            m_rigidbody = GetComponent<Rigidbody2D>();
        }

        private void LateUpdate()
        {
            if (m_targetObject)
            {
                //transform.rotation = Quaternion.identity;

                float angleDeg = Mathf.Atan2(m_rigidbody.velocity.y, m_rigidbody.velocity.x) * Mathf.Rad2Deg;
                float offset = m_offsetRotate;
                float val = m_targetObject.transform.rotation.eulerAngles.z;

                m_targetObject.transform.Rotate(0, 0, transform.lossyScale.y * (angleDeg + offset - val));
            }
        }
    }
}
