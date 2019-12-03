using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Proto2D
{
    [RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
    public class TriggerBackgroundSet : MonoBehaviour
    {
        public Sprite m_backgroundSprite;

        private SpriteRenderer m_bgSpriteRenderer;

        private void Start()
        {
            Background bg = GameObject.FindObjectOfType<Background>();
            if (bg)
            {
                m_bgSpriteRenderer = bg.GetComponent<SpriteRenderer>();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                // 背景画像の設定
                m_bgSpriteRenderer.sprite = m_backgroundSprite;

                // 一度だけ行う
                GetComponent<BoxCollider2D>().enabled = false;
            }
        }

        private void OnDrawGizmos()
        {
            BoxCollider2D collider = GetComponent<BoxCollider2D>();
            if (collider && collider.enabled)
            {
                Gizmos.color = new Color(1, 1, 0, .2f);
                Gizmos.DrawCube(collider.bounds.center, collider.bounds.size);
            }
        }
    }
}
