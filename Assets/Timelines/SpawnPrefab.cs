using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Proto2D
{
    [System.Serializable]
    public class SpawnPrefab : PlayableAsset
    {
        [SerializeField]
        ExposedReference<GameObject> m_prefab;

        [SerializeField]
        ExposedReference<Transform> m_spawnPosition;

        // Factory method that generates a playable based on this asset
        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            SpawnPrefabBehaviour behaviour = new SpawnPrefabBehaviour();

            behaviour.prefab = m_prefab.Resolve(graph.GetResolver());
            behaviour.spawnPosition = m_spawnPosition.Resolve(graph.GetResolver());

            ScriptPlayable<SpawnPrefabBehaviour> playable = ScriptPlayable<SpawnPrefabBehaviour>.Create(graph, behaviour);

            return playable;
        }
    }
}
