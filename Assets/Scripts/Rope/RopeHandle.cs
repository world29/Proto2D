using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Proto2D
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class RopeHandle : MonoBehaviour
    {
        [SerializeField]
        UnityEvent m_OnGrabbed;

        [SerializeField]
        UnityEvent m_OnReleased;

        public void Grab()
        {
            m_OnGrabbed.Invoke();
        }

        public void Release()
        {
            m_OnReleased.Invoke();
        }
    }
}
