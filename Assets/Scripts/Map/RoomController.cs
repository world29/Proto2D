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
        [EnumFlags]
        public StagePhaseFlag m_stagePhaseFlag = StagePhaseFlag.Phase1 | StagePhaseFlag.Phase2 | StagePhaseFlag.Phase3;

        public Transform enemySpawnPosition;
        public List<EnemyBehaviour> enemyPrefabs;

        [HideInInspector] public Tilemap PrimaryTilemap { get { return getPrimaryTilemap(); } }
        [HideInInspector] public Vector3Int Size { get { return getPrimaryTilemap().size; } }
        [HideInInspector] public Vector3 CellSize { get { return getPrimaryTilemap().cellSize; } }
        [HideInInspector] public Vector3Int Origin { get { return getPrimaryTilemap().origin; } }
        // タイルマップの左下端から (0,0) へのオフセット
        //deprecated: インスタンス化するまで CellToLocal 画適切な値を返さない
        [HideInInspector] public Vector3 OriginToCenter { get { return -getPrimaryTilemap().CellToLocal(getPrimaryTilemap().origin); } }

        private Tilemap m_primaryTilemap;

        private void Awake()
        {
            getPrimaryTilemap();
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

        private Tilemap getPrimaryTilemap()
        {
            if (m_primaryTilemap == null)
            {
                Tilemap tilemap = GetComponentInChildren<Tilemap>();
                if (tilemap)
                {
                    m_primaryTilemap = tilemap;
                    m_primaryTilemap.CompressBounds();
                }
            }
            return m_primaryTilemap;
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
            if (m_primaryTilemap)
            {
                var minWorld = m_primaryTilemap.CellToWorld(m_primaryTilemap.cellBounds.min);
                var maxWorld = m_primaryTilemap.CellToWorld(m_primaryTilemap.cellBounds.max);

                Bounds bounds = new Bounds();
                bounds.SetMinMax(minWorld, maxWorld);

                Gizmos.color = new Color(1, 1, 0, .3f);
                Gizmos.DrawCube(bounds.center, bounds.size);
            }
        }
    }
}

