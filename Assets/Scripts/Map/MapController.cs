﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    public class MapController : MonoBehaviour
    {
        public List<RoomController> startRoomPrefabs;
        public List<RoomController> normalRoomPrefabs;

        public PlayerController playerPrefab;

        public Vector2 spawnAreaSize;
        public float spawnAreaOffset = 10;

        private Vector2 m_edgePosition = Vector2.zero;
        private SpawnArea m_spawnArea;

        private void Awake()
        {
            m_spawnArea = new SpawnArea(spawnAreaSize);
            m_spawnArea.Update(Vector3.zero, spawnAreaOffset);
        }

        void Start()
        {
            // スタート部屋を生成
            spawnNextRoom(getRandomStartRoom());

            GameController.Instance.OnMapInitialized();
        }

        void LateUpdate()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player)
            {
                m_spawnArea.Update(player.transform.position, spawnAreaOffset);
            }

            // プレイヤーの上方向に部屋を生成
            while (m_spawnArea.IsIntersects(m_edgePosition))
            {
                spawnNextRoom(getRandomNormalRoom());
            }
        }

        void spawnNextRoom(RoomController prefab)
        {
            // 部屋を仮の位置に生成してから、適切な位置に移動する。
            // 部屋のサイズはタイルマップを生成した後でないと取得できないため。
            // そのため、MonoBehaviour.Awake() や Start() では位置を使用した処理 (オブジェクトのスポーンなど) ができない。
            RoomController room = GameObject.Instantiate(prefab, gameObject.transform);

            float roomHeight = room.Size.y * room.CellSize.y;
            room.gameObject.transform.Translate(0, m_edgePosition.y + room.OriginToCenter.y, 0);

            if (room.flipEnabled)
            {
                float dirX = Mathf.Sign(Random.Range(-1, 1));
                room.gameObject.transform.localScale = new Vector3(dirX, 1, 1);
            }

            // 部屋を適切な位置に移動した後、スポーン処理を呼び出す。
            room.SpawnEntities();

            m_edgePosition.y += roomHeight;
        }

        RoomController getRandomStartRoom()
        {
            if (startRoomPrefabs.Count > 0)
            {
                int index = Random.Range(0, startRoomPrefabs.Count);
                return startRoomPrefabs[index];
            }
            return null;
        }

        RoomController getRandomNormalRoom()
        {
            if (normalRoomPrefabs.Count > 0)
            {
                int index = Random.Range(0, normalRoomPrefabs.Count);
                Debug.Log(index);
                return normalRoomPrefabs[index];
            }
            return null;
        }

        private void OnDrawGizmos()
        {
            // マップ上端
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(m_edgePosition, .5f);

            // スポーン領域
            Gizmos.color = new Color(0, 0, 1, .2f);
            Gizmos.DrawCube(m_spawnArea.bounds.center, m_spawnArea.bounds.size);
        }

        struct SpawnArea
        {
            public Bounds bounds;

            public SpawnArea(Vector2 size)
            {
                bounds = new Bounds(Vector3.zero, size);
            }

            public void Update(Vector3 targetPosition, float verticalOffset)
            {
                bounds.center = new Vector3(0, targetPosition.y + verticalOffset, 0);
            }

            public bool IsIntersects(Vector3 position)
            {
                Bounds targetBounds = new Bounds(position, Vector3.one);
                return bounds.Intersects(targetBounds);
            }
        }
    }
}
