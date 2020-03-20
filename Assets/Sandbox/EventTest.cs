using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Proto2D
{
    public class EventTest : MonoBehaviour
    {
        public UnityEvent m_enterEvent;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                m_enterEvent.Invoke();
            }
        }

        public void Print()
        {
            Debug.Log("test function");
        }
    }
}
