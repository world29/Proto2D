using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Proto2D
{
    [System.Serializable]
    public class PlayerInputDisablerAsset : PlayableAsset
    {
        // Factory method that generates a playable based on this asset
        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            PlayerInputDisablerBehaviour behaviour = new PlayerInputDisablerBehaviour();

            behaviour.playerObject = GameObject.FindGameObjectWithTag("Player");

            ScriptPlayable<PlayerInputDisablerBehaviour> playable = ScriptPlayable<PlayerInputDisablerBehaviour>.Create(graph, behaviour);

            return playable;
        }
    }
}
