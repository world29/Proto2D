using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UniRx;
using UniRx.Triggers;
using TMPro;

namespace Proto2D
{
    public class SoundConfigDialog : Dialog
    {
        [SerializeField]
        MixLevels m_mixLevels;

        [SerializeField]
        Slider m_masterVolumeSlider;
        [SerializeField]
        Slider m_musicVolumeSlider;
        [SerializeField]
        Slider m_sfxVolumeSlider;

        private void Start()
        {
            m_masterVolumeSlider.value = m_mixLevels.masterVolume;
            m_musicVolumeSlider.value = m_mixLevels.musicVolume;
            m_sfxVolumeSlider.value = m_mixLevels.sfxVolume;
        }
    }
}
