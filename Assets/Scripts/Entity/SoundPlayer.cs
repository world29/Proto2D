using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    public class SoundPlayer : MonoBehaviour
    {
        public void PlaySound(AudioClip clip)
        {
            if (Globals.SoundPlayer.Instance)
            {
                Globals.SoundPlayer.Instance.Play(clip);
            }
            else
            {
                Debug.LogWarning("Globals.SoundPlayer is not exists");
            }
        }
    }
}
