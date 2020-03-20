using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D.Globals
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundPlayer : SingletonMonoBehaviour<SoundPlayer>
    {
        private AudioSource audioSource;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.spatialBlend = 0; // 2D サウンド
            audioSource.playOnAwake = false;
        }

        public void Play(AudioClip clip)
        {
            Debug.Assert(audioSource != null, "SoundPlayer.Start() より先に呼び出してはいけません");

            audioSource.PlayOneShot(clip);
        }
    }
}
