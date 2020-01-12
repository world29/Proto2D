using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    [RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
    public class TriggerEffect : MonoBehaviour
    {
        public GameObject m_effectPrefab;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Transform root = Camera.main.transform;

                // メインカメラの子オブジェクトを全削除
                for (int i = 0; i < root.childCount; i++)
                {
                    Transform child = root.GetChild(i);
                    Destroy(child.gameObject);
                }
                root.DetachChildren();

                // パーティクルプレハブをメインカメラの子に設定
                Instantiate(m_effectPrefab, root);

                // BGM の変更は一度だけ行う
                GetComponent<BoxCollider2D>().enabled = false;
            }
        }
    }
}
