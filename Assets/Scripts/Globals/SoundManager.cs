using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    /// <summary>
    /// サウンドマネージャ
    /// BGM の再生、停止機能を提供
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class SoundManager : SingletonMonoBehaviour<SoundManager>
    {
        AudioSource m_audioSource;

        private Coroutine m_fadeOutCoroutine;

        private void Start()
        {
            m_audioSource = GetComponent<AudioSource>();
        }

        public void Play(AudioClip audioClip)
        {
            if (m_fadeOutCoroutine != null)
            {
                StopCoroutine(m_fadeOutCoroutine);
            }

            m_audioSource.clip = audioClip;
            m_audioSource.Play();
            m_audioSource.volume = 1;
        }

        public void Stop(float fadeDuration)
        {
            m_fadeOutCoroutine = StartCoroutine(FadeOut(fadeDuration));
        }

        private IEnumerator FadeOut(float duration)
        {
            float maxVolume = m_audioSource.volume;
            float endTime = Time.timeSinceLevelLoad + duration;

            while (Time.timeSinceLevelLoad < endTime)
            {
                float percentage = (endTime - Time.timeSinceLevelLoad) / duration;

                m_audioSource.volume = percentage * maxVolume;

                yield return new WaitForEndOfFrame();
            }

            m_audioSource.volume = 0;
            m_audioSource.Stop();
        }

    }
}
