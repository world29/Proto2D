using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

namespace Proto2D
{
    [RequireComponent(typeof(Laser), typeof(Rigidbody2D), typeof(Collider2D))]
    public class Absorbable : MonoBehaviour
    {
        [SerializeField]
        float m_delay;

        private void Awake()
        {
            GetComponent<Laser>().enabled = false;
        }

        private void Start()
        {
            Observable.Timer(TimeSpan.FromSeconds(m_delay))
                .Subscribe(_ => BeginAbsorb())
                .AddTo(gameObject);
        }

        private void BeginAbsorb()
        {
            GetComponent<Rigidbody2D>().isKinematic = true;
            GetComponent<Collider2D>().isTrigger = true;
            GetComponent<Laser>().enabled = true;
        }
    }
}
