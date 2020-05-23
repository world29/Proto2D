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

        public void SetMasterVolume(float level)
        {
            m_masterMixer.SetFloat("masterVolume", Mathf.Lerp(-80f, 0f, level));
        }

        public void SetMusicVolume(float level)
        {
            m_masterMixer.SetFloat("musicVolume", Mathf.Lerp(-80f, 0f, level));
        }

        public void SetSfxVolume(float level)
        {
            m_masterMixer.SetFloat("sfxVolume", Mathf.Lerp(-80f, 0f, level));
        }
    }
}
