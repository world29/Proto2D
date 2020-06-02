using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Proto2D
{
    [System.Serializable]
    public class PlayerCommandJump : PlayableAsset
    {
        // Factory method that generates a playable based on this asset
        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            PlayerCommandJumpBehaviour behaviour = new PlayerCommandJumpBehaviour();

            behaviour.playerObject = GameObject.FindGameObjectWithTag("Player");

            ScriptPlayable<PlayerCommandJumpBehaviour> playable = ScriptPlayable<PlayerCommandJumpBehaviour>.Create(graph, behaviour);

            return playable;
        }
    }
}
