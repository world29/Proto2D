using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class PlayerUncontrollable : PlayableAsset
{
    [SerializeField]
    RuntimeAnimatorController animatorController;

    // Factory method that generates a playable based on this asset
    public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
    {
        PlayerUncontrollableBehaviour behaviour = new PlayerUncontrollableBehaviour();

        behaviour.playerObject = go;
        behaviour.animatorController = animatorController;

        ScriptPlayable<PlayerUncontrollableBehaviour> playable = ScriptPlayable<PlayerUncontrollableBehaviour>.Create(graph, behaviour);

        return playable;
    }
}
