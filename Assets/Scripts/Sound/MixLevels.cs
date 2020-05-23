using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Proto2D
{
    public class MixLevels : MonoBehaviour
    {
        [SerializeField]
        AudioMixer m_masterMixer;

        public float masterVolume { get; private set; }
        public float musicVolume { get; private set; }
        public float sfxVolume { get; private set; }

        public void Init(float masterVol, float musicVol, float sfxVol)
        {
            SetMasterVolume(masterVol);
            SetMusicVolume(musicVol);
            SetSfxVolume(sfxVol);
        }

        public void SetMasterVolume(float level)
        {
            masterVolume = Mathf.Clamp01(level);
            m_masterMixer.SetFloat("masterVolume", Mathf.Lerp(-80f, 0f, masterVolume));
        }

        public void SetMusicVolume(float level)
        {
            musicVolume = Mathf.Clamp01(level);
            m_masterMixer.SetFloat("musicVolume", Mathf.Lerp(-80f, 0f, musicVolume));
        }

        public void SetSfxVolume(float level)
        {
            sfxVolume = Mathf.Clamp01(level);
            m_masterMixer.SetFloat("sfxVolume", Mathf.Lerp(-80f, 0f, sfxVolume));
        }
    }
}
