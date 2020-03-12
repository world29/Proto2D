using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    public class CharacterManager : SingletonMonoBehaviour<CharacterManager>
    {
        public GameObject m_playerPrefab;

        private GameObject m_playerInstance;

        void Start()
        {
        }

        void Update()
        {
        }

        public GameObject GetPlayer()
        {
            if (m_playerInstance == null)
            {
                Vector3 pos = Vector3.zero;
                Quaternion rot = Quaternion.identity;

                // シーン上に存在する PlayerSpawner の位置に生成する
                var playerSpawner = GameObject.FindGameObjectWithTag("PlayerSpawner");
                if (playerSpawner)
                {
                    pos = playerSpawner.transform.position;
                }

                m_playerInstance = GameObject.Instantiate(m_playerPrefab, pos, rot);
            }

            return m_playerInstance;
        }
    }
}
