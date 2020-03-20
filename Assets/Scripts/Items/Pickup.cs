﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Proto2D
{
    // 拾うことが出来るアイテムの共通基底クラス
    [RequireComponent(typeof(Rigidbody2D), typeof(Collision2D))]
    public class Pickup : MonoBehaviour
    {
        // 非推奨な UnityEngine.Component.rigidbody を無効化するため new で宣言
        [HideInInspector]
        public new Rigidbody2D rigidbody { get { return GetComponent<Rigidbody2D>(); } }

        public UnityEvent OnPickup;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                OnPickup.Invoke();

                Destroy(gameObject);
            }
        }
    }
}