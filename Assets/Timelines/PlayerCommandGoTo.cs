using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Proto2D
{
    [System.Serializable]
    public class PlayerCommandGoTo : PlayableAsset
    {
        [SerializeField]
        ExposedReference<Transform> m_targetPosition;

        // Factory method that generates a playable based on this asset
        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            PlayerCommandGoToBehaviour behaviour = new PlayerCommandGoToBehaviour();

            behaviour.playerObject = GameObject.FindGameObjectWithTag("Player");
            behaviour.targetPosition = m_targetPosition.Resolve(graph.GetResolver());

            ScriptPlayable<PlayerCommandGoToBehaviour> playable = ScriptPlayable<PlayerCommandGoToBehaviour>.Create(graph, behaviour);

            return playable;
        }
    }
}
