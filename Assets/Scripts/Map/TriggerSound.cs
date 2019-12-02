using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    [RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
    public class TriggerSound : MonoBehaviour
    {
        public AudioClip m_sound;
        public float m_fadeTime = 1;

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
                    StartCoroutine(ChangeAudio(audioSource, m_sound, m_fadeTime));
                }

                // BGM の変更は一度だけ行う
                GetComponent<BoxCollider2D>().enabled = false;
            }
        }

        IEnumerator ChangeAudio(AudioSource source, AudioClip clipToPlay, float fadeTime)
        {
            // フェードアウト
            float timeEndFadeOut = Time.timeSinceLevelLoad + fadeTime;
            while (Time.timeSinceLevelLoad < timeEndFadeOut)
            {
                source.volume = (timeEndFadeOut - Time.timeSinceLevelLoad) / fadeTime;

                yield return new WaitForEndOfFrame();
            }

            source.Stop();
            source.clip = clipToPlay;
            source.Play();

            // フェードイン
            float timeEndFadeIn = Time.timeSinceLevelLoad + fadeTime;
            while (Time.timeSinceLevelLoad < timeEndFadeIn)
            {
                source.volume = 1 - ((timeEndFadeIn - Time.timeSinceLevelLoad) / fadeTime);

                yield return new WaitForEndOfFrame();
            }
        }
    }
}
