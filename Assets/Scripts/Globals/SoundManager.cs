using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

namespace Proto2D.Globals
{
    public class SoundManager : SingletonMonoBehaviour<SoundManager>
    {
        [SerializeField]
        AudioSource m_musicSource;

        [SerializeField]
        AudioSource m_sfxSource;

        [SerializeField]
        Dialog m_soundMenuDialog;

        [SerializeField]
        MixLevels m_mixLevels;

        [SerializeField]
        float m_fadeOutDuration;

        [SerializeField]
        float m_fadeInDuration;

        private IDisposable m_fadeInOutHandle;
        private float m_masterVolumeSave;

        private void Start()
        {
            LoadSoundConfig();
        }

        // SE の再生
        public void Play(AudioClip clip)
        {
            m_sfxSource.PlayOneShot(clip);
        }

        // BGM の再生
        public void PlayMusic(AudioClip clip)
        {
            m_musicSource.clip = clip;
            m_musicSource.Play();
            m_musicSource.volume = 1;
        }

        // BGM の停止
        public void StopMusic()
        {
            StartCoroutine(StopMusic(m_fadeOutDuration));
        }

        public IEnumerator StopMusic(float fadeDuration)
        {
            yield return FadeOutMusic(fadeDuration);

            m_musicSource.clip = null;
        }

        private IEnumerator FadeOutMusic(float duration)
        {
            float maxVolume = m_musicSource.volume;
            float endTime = Time.time + duration;

            while (Time.time < endTime)
            {
                float percentage = (endTime - Time.time) / duration;

                m_musicSource.volume = percentage * maxVolume;

                yield return new WaitForEndOfFrame();
            }

            m_musicSource.volume = 0;
            m_musicSource.Stop();
        }

        public void FadeOut()
        {
            FadeOutImpl(m_fadeOutDuration);
        }

        private void FadeOutImpl(float duration)
        {
            if (m_fadeInOutHandle != null)
            {
                m_fadeInOutHandle.Dispose();
            }

            const float intervalMs = 50f;
            float count = duration * 1000 / intervalMs;

            m_masterVolumeSave = m_mixLevels.masterVolume;

            m_fadeInOutHandle = Observable
                .Timer(TimeSpan.Zero, TimeSpan.FromMilliseconds(intervalMs))
                .TakeWhile(l => l <= count)
                .Subscribe(l =>
                {
                    var level = Mathf.Lerp(m_masterVolumeSave, 0, l / count);
                    m_mixLevels.SetMasterVolume(level);
                });
        }

        public void FadeIn()
        {
            FadeInImpl(m_fadeInDuration);
        }

        private void FadeInImpl(float duration)
        {
            if (m_fadeInOutHandle != null)
            {
                m_fadeInOutHandle.Dispose();
            }

            const float intervalMs = 50f;
            float count = duration * 1000 / intervalMs;

            m_fadeInOutHandle = Observable
                .Timer(TimeSpan.Zero, TimeSpan.FromMilliseconds(intervalMs))
                .TakeWhile(l => l <= count)
                .Subscribe(l =>
                {
                    var level = Mathf.Lerp(0, m_masterVolumeSave, l / count);
                    m_mixLevels.SetMasterVolume(level);
                });
        }

        public void OpenSoundMenu()
        {
            m_soundMenuDialog.Open();
        }

        private void LoadSoundConfig()
        {
            m_mixLevels.Init(
                PlayerPrefs.GetFloat("masterVolume", 1f),
                PlayerPrefs.GetFloat("musicVolume", 1f),
                PlayerPrefs.GetFloat("sfxVolume", 1f));
        }

        public void SaveSoundConfig()
        {
            PlayerPrefs.SetFloat("masterVolume", m_mixLevels.masterVolume);
            PlayerPrefs.SetFloat("musicVolume", m_mixLevels.musicVolume);
            PlayerPrefs.SetFloat("sfxVolume", m_mixLevels.sfxVolume);

            PlayerPrefs.Save();
        }
    }
}
