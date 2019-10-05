using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

namespace Proto2D
{
    public class RoomController : MonoBehaviour
    {
        public bool flipEnabled = false;

        public Transform enemySpawnPosition;
        public List<EnemyBehaviour> enemyPrefabs;

        [HideInInspector] public Vector3Int Size { get { return primaryTilemap.size; } }
        [HideInInspector] public Vector3 CellSize { get { return primaryTilemap.cellSize; } }
        // タイルマップの左下端から (0,0) へのオフセット
        [HideInInspector] public Vector3 OriginToCenter { get { return -primaryTilemap.CellToLocal(primaryTilemap.origin); } }

        private Tilemap primaryTilemap;

        private void Awake()
        {
            Tilemap tilemap = GetComponentInChildren<Tilemap>();
            if (tilemap)
            {
                primaryTilemap = tilemap;
                primaryTilemap.CompressBounds();
            }
        }

        public void SpawnEntities()
        {
            // 敵をランダムに選んでスポーン
            EnemyBehaviour prefab = getRandomEnemy();
            if (prefab)
            {
                Debug.Assert(enemySpawnPosition != null);
                EnemyBehaviour enemy = GameObject.Instantiate(prefab, enemySpawnPosition);
            }
        }

        private EnemyBehaviour getRandomEnemy()
        {
            if (enemyPrefabs.Count > 0)
            {
                int index = Random.Range(0, enemyPrefabs.Count);
                return enemyPrefabs[index];
            }
            return null;
        }

        private void OnDrawGizmos()
        {
            if (primaryTilemap)
            {
                var minWorld = primaryTilemap.CellToWorld(primaryTilemap.cellBounds.min);
                var maxWorld = primaryTilemap.CellToWorld(primaryTilemap.cellBounds.max);

                Bounds bounds = new Bounds();
                bounds.SetMinMax(minWorld, maxWorld);

                Gizmos.color = new Color(1, 1, 0, .3f);
                Gizmos.DrawCube(bounds.center, bounds.size);
            }
        }
    }
}

