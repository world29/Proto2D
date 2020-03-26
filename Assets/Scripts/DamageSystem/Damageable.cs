﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    [RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
    public class Damageable : MonoBehaviour
    {
        [SerializeField, EnumFlags]
        public DamageTypeFlag m_damageTypeFlag;

        [SerializeField, Header("ダメージを受けたことを伝えるオブジェクト。null の場合はルートオブジェクト")]
        public GameObject m_owner;

        private void Awake()
        {
            if (m_owner == null)
            {
                m_owner = transform.root.gameObject;
            }

            // recenver が IDamageReceiver を実装していない
            if (m_owner.GetComponent<IDamageReceiver>() == null)
            {
                Debug.LogWarningFormat("{0} can not receive DamageReceiveEvent", m_owner.name);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // nop
        }

        private void OnDrawGizmos()
        {
            if (!enabled) return;

            // ダメージ判定の矩形描画
            BoxCollider2D collider = GetComponent<BoxCollider2D>();
            if (collider.enabled)
            {
                Gizmos.color = new Color(0, 1, 0, .2f);
                Gizmos.DrawCube(collider.bounds.center, collider.bounds.size);
            }
        }
    }
}
