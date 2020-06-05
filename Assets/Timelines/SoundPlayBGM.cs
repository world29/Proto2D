using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Proto2D
{
    [System.Serializable]
    public class SoundPlayBGM : PlayableAsset
    {
        [SerializeField]
        AudioClip m_audioClip;

        // Factory method that generates a playable based on this asset
        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            SoundPlayBGMBehaviour behaviour = new SoundPlayBGMBehaviour();

            behaviour.audioClip = m_audioClip;

            ScriptPlayable<SoundPlayBGMBehaviour> playable = ScriptPlayable<SoundPlayBGMBehaviour>.Create(graph, behaviour);

            return playable;
        }
    }
}
