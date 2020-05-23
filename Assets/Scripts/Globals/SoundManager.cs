using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D.Globals
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundManager : SingletonMonoBehaviour<SoundManager>
    {
        [SerializeField]
        Dialog m_soundMenuDialog;

        [SerializeField]
        MixLevels m_mixLevels;

        private AudioSource audioSource;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.spatialBlend = 0; // 2D サウンド
            audioSource.playOnAwake = false;

            LoadSoundConfig();
        }

        public void Play(AudioClip clip)
        {
            Debug.Assert(audioSource != null, "SoundManager.Start() より先に呼び出してはいけません");

            audioSource.PlayOneShot(clip);
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
