using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    [RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
    public class TriggerSoundPlay : MonoBehaviour
    {
        public AudioClip m_audioClip;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                if (SoundManager.Instance)
                {
                    SoundManager.Instance.Play(m_audioClip);
                }

                // BGM の変更は一度だけ行う
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
