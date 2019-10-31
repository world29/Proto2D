﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Proto2D
{
    public class RoomSpawner : MonoBehaviour
    {
        public List<RoomController> m_roomPrefabs;

        public Bounds Boundary { get { return new Bounds(transform.position, m_localBounds.size); } }
        private Bounds m_localBounds = new Bounds(Vector3.zero, Vector3.one);

        private TilemapController m_tilemapController;

        private void Awake()
        {
            m_tilemapController = FindObjectOfType<TilemapController>();
        }

        void Start()
        {
            if (m_roomPrefabs.Count == 0) return;

            if (m_tilemapController.m_tilemapBoundary.Intersects(Boundary))
            {
                spawnNextRoom();
            }
        }

        void LateUpdate()
        {
            if (m_tilemapController.m_tilemapBoundary.Intersects(Boundary))
            {
                spawnNextRoom();
            }
        }

        // 部屋を生成して、自身の位置を生成した部屋の上端に移動する
        private void spawnNextRoom()
        {
            Debug.Assert(m_roomPrefabs.Count > 0);

            int roomIndex = Random.Range(0, m_roomPrefabs.Count);
            RoomController rc = m_roomPrefabs[roomIndex];

            // タイルマップ原点が部屋の中心軸上にあるかチェック
            Tilemap tilemap = rc.PrimaryTilemap;
            if (Mathf.Abs(tilemap.origin.x) != (tilemap.size.x / 2))
            {
                Debug.LogWarningFormat("horizontal center of tilemap must be zero. {0}", rc.gameObject.name);
            }

            // 生成済みの部屋の上端とつながるように新たな部屋を生成する
            float bottomToCenterY = -tilemap.origin.y * tilemap.cellSize.y;
            Vector3 spawnPosition = new Vector3(0, bottomToCenterY, 0);
            spawnPosition += transform.position;

            // タイルをシーンにコピーしてから、部屋インスタンスを生成する
            Vector3Int copyPos = new Vector3Int(Mathf.FloorToInt(spawnPosition.x / tilemap.cellSize.x), Mathf.FloorToInt(spawnPosition.y / tilemap.cellSize.y), 0);
            foreach (Tilemap tm in rc.GetComponentsInChildren<Tilemap>())
            {
                m_tilemapController.CopyTilesImmediate(tm, copyPos);
            }

            //HACK: コピー元の Grid 位置をコピー先の Grid 位置にあわせる
            //spawnPosition += new Vector3(.5f, .5f, 0);

            //Destroy(rc.GetComponentInChildren<Grid>().gameObject);
            var spawnedRoom = GameObject.Instantiate(rc, spawnPosition, Quaternion.identity);
            Destroy(spawnedRoom.GetComponentInChildren<Grid>().gameObject);

            //TODO:
            spawnedRoom.SpawnEntities();

            /* TODO:
            if (room.flipEnabled)
            {
                float dirX = Mathf.Sign(Random.Range(-1, 1));
                room.gameObject.transform.localScale = new Vector3(dirX, 1, 1);
            }
            */

            float roomHeight = tilemap.size.y * tilemap.cellSize.y;
            transform.Translate(0, roomHeight, 0);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1, 0, 0, .5f);
            Gizmos.DrawCube(transform.position, m_localBounds.size);
        }
    }
}
