using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.NewData.Scripts
{
    // アニメーションイベントから SE を再生するのに使う
    public class InGameSePlayer : MonoBehaviour
    {
        public void PlaySe(AudioClip clip)
        {
            SoundManager.PlaySe(clip);
        }
    }
}
