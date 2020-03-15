using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    // ランダムな位置と向きにミサイルを生成する
    public class MissleSpawner : MonoBehaviour
    {
        public GameObject m_prefab;
        public Rect m_spawnArea;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                var x = Random.Range(m_spawnArea.min.x, m_spawnArea.max.x);
                var y = Random.Range(m_spawnArea.min.y, m_spawnArea.max.y);
                var angle = Random.Range(0, 360);

                GameObject.Instantiate(m_prefab, new Vector3(x, y, 0), Quaternion.Euler(0, 0, angle));
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0, 1, 1, .3f);
            Gizmos.DrawCube(m_spawnArea.center, m_spawnArea.size);
        }
    }
}
