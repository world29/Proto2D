using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.NewData.Scripts
{
    // タイムラインから BGM を再生するのに使う
    public class InGameBgmPlayer : MonoBehaviour
    {
        [SerializeField]
        private float fadeTime = 4f;

        public void PlayBgm(string assetAddress)
        {
            SoundManager.PlayBgm(assetAddress, fadeTime);
        }
    }
}
