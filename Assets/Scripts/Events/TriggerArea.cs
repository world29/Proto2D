using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UniRx;
using System;

namespace Proto2D
{
    [RequireComponent(typeof(Collider2D))]
    public class TriggerArea : MonoBehaviour
    {
        [SerializeField]
        UnityEvent m_OnEnter;

        [SerializeField, Header("起動回数の上限")]
        int m_activationLimit = 1;

        [SerializeField, Header("起動ごとのインターバル (秒)")]
        Vector2 m_interval = Vector2.one;

        private new Collider2D collider { get { return GetComponent<Collider2D>(); } }

        int m_activatedCount;

        private void Start()
        {
            m_activatedCount = 0;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                m_OnEnter.Invoke();
            }
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            OnCollisionEvent(collider);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            OnCollisionEvent(collision.collider);
        }

        private void OnTriggerStay2D(Collider2D collider)
        {
            OnCollisionEvent(collider);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            OnCollisionEvent(collision.collider);
        }

        protected virtual void OnCollisionEvent(Collider2D collider)
        {
            if (collider.gameObject.CompareTag("Player"))
            {
                m_OnEnter.Invoke();

                m_activatedCount++;

                // 起動回数の上限に達したら削除
                if (m_activatedCount >= m_activationLimit)
                {
                    Destroy(gameObject);
                }
                else
                {
                    // インターバルの間は無効化する
                    var interval = UnityEngine.Random.Range(m_interval.x, m_interval.y);
                    Observable.Timer(TimeSpan.FromSeconds(interval))
                        .Subscribe(_ => gameObject.SetActive(true));

                    gameObject.SetActive(false);
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0, 1, 1, .3f);
            Gizmos.DrawCube(collider.bounds.center, collider.bounds.size);
        }
    }
}
