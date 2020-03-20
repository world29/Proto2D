using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Proto2D
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class TriggerArea : MonoBehaviour
    {
        [SerializeField]
        UnityEvent m_OnEnter;

        private new BoxCollider2D collider { get { return GetComponent<BoxCollider2D>(); } }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                m_OnEnter.Invoke();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                m_OnEnter.Invoke();
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0, 1, 1, .3f);
            Gizmos.DrawCube(collider.bounds.center, collider.bounds.size);
        }
    }
}
