using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    [RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
    public class TriggerSound : MonoBehaviour
    {
        public AudioClip m_sound;

        private GameController m_gameController;

        private void Awake()
        {
            GameObject go = GameObject.FindGameObjectWithTag("GameController");
            if (go)
            {
                m_gameController = go.GetComponent<GameController>();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                AudioSource audioSource = m_gameController.GetComponent<AudioSource>();
                if (audioSource)
                {
                    audioSource.Stop();
                    audioSource.clip = m_sound;
                    audioSource.Play();
                }
            }
        }
    }
}
