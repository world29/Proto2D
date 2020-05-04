using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UniRx;
using System;

namespace Proto2D
{
    [RequireComponent(typeof(Collider2D))]
    public class TriggerDialog : MonoBehaviour
    {
        [SerializeField]
        Canvas m_dialogPrefab;

        private Canvas m_dialogClone;

        const string TARGET_TAG = "Player";

        private void Start()
        {
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.gameObject.CompareTag(TARGET_TAG))
            {
                m_dialogClone = GameObject.Instantiate(m_dialogPrefab);
            }
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            if (collider.gameObject.CompareTag(TARGET_TAG))
            {
                if (m_dialogClone != null)
                {
                    Destroy(m_dialogClone.gameObject);
                }
            }
        }
    }
}
