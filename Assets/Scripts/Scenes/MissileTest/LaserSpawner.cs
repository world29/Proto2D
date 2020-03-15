using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    // ランダムな位置にレーザーを生成し、初速を与える
    public class LaserSpawner : MonoBehaviour
    {
        public GameObject m_prefab;
        public Rect m_spawnArea;
        public float m_initialSpeed;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                var x = Random.Range(m_spawnArea.min.x, m_spawnArea.max.x);
                var y = Random.Range(m_spawnArea.min.y, m_spawnArea.max.y);
                var angle = Random.Range(0, 360);

                var go = GameObject.Instantiate(m_prefab, new Vector3(x, y, 0), Quaternion.identity);
                var laser = go.GetComponent<Laser>();
                Debug.Assert(laser);
                laser.initialVelocity = Quaternion.Euler(0, 0, angle) * (Vector3.right * m_initialSpeed);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0, 1, 1, .3f);
            Gizmos.DrawCube(m_spawnArea.center, m_spawnArea.size);
        }
    }
}
