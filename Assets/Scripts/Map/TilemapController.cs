using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

namespace Proto2D
{
    public class TilemapController : MonoBehaviour
    {
        public Tilemap m_tilemapObstacles;
        public Tilemap m_tilemapBackground;
        public Tilemap m_tilemapBackgroundDeco;
        public Tilemap m_tilemapBackgroundAdd;

        //TODO: GameController.WorldBoundary プロパティとする
        public Bounds m_tilemapBoundary;

        public PlayerController m_playerPrefab;

        private void Start()
        {
            if (m_playerPrefab)
            {
                Instantiate(m_playerPrefab);
            }
        }

        private void LateUpdate()
        {
            Vector2 cameraPosition = Camera.main.transform.position;
            m_tilemapBoundary.center = cameraPosition;
        }

        // CopyTilesImmediate

        public void CopyTilesImmediate(Tilemap sourceTilemap, Vector3Int destPos)
        {
            Dictionary<Vector3Int, TileBase> tiles = new Dictionary<Vector3Int, TileBase>();

            foreach (var position in sourceTilemap.cellBounds.allPositionsWithin)
            {
                if (sourceTilemap.HasTile(position))
                {
                    tiles.Add(destPos + position, sourceTilemap.GetTile(position));
                }
            }

            // SortedLayer からコピー先タイルマップを判定する
            TilemapRenderer tr = sourceTilemap.GetComponent<TilemapRenderer>();
            switch (tr.sortingLayerName)
            {
                case "ObstacleTile":
                    m_tilemapObstacles.SetTiles(tiles.Keys.ToArray(), tiles.Values.ToArray());
                    break;
                case "BackgroundTile":
                    m_tilemapBackground.SetTiles(tiles.Keys.ToArray(), tiles.Values.ToArray());
                    break;
                case "BackgroundDecorationTile":
                    m_tilemapBackgroundDeco.SetTiles(tiles.Keys.ToArray(), tiles.Values.ToArray());
                    break;
                case "BackgroundTile2":
                    m_tilemapBackgroundAdd.SetTiles(tiles.Keys.ToArray(), tiles.Values.ToArray());
                    break;
                default:
                    Debug.Assert(false, "tilemap has unexpected sorting layer");
                    break;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0, 1, 1, .2f);
            Gizmos.DrawCube(m_tilemapBoundary.center, m_tilemapBoundary.size);
        }
    }
}
