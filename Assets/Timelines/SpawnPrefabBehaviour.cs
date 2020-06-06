using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Proto2D
{
    public class SpawnPrefabBehaviour : PlayableBehaviour
    {
        public GameObject prefab;
        public Transform spawnPosition;

        private GameObject m_spawnedObject;

        // Called when the owning graph starts playing
        public override void OnGraphStart(Playable playable)
        {
        }

        // Called when the owning graph stops playing
        public override void OnGraphStop(Playable playable)
        {
        }

        // Called when the state of the playable is set to Play
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            m_spawnedObject = GameObject.Instantiate(prefab);
            m_spawnedObject.transform.position = spawnPosition.position;

            // スポーンしたオブジェクトがエネミーの場合、トラック終了までビヘイビアツリーの更新を停止する
            var enemyBehaviour = m_spawnedObject.GetComponent<EnemyBehaviour>();
            if (enemyBehaviour)
            {
                enemyBehaviour.behaviourTreeUpdatable = false;
            }
        }

        // Called when the state of the playable is set to Paused
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if(m_spawnedObject)
            {
                var enemyBehaviour = m_spawnedObject.GetComponent<EnemyBehaviour>();
                if (enemyBehaviour)
                {
                    enemyBehaviour.behaviourTreeUpdatable = true;
                }
            }
        }

        // Called each frame while the state is set to Play
        public override void PrepareFrame(Playable playable, FrameData info)
        {
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
        }
    }
}
