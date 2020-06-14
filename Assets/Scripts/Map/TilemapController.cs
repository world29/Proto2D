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
        public Tilemap m_tilemapBackgroundShade;
        public Tilemap m_tilemapBackgroundAdd;
        public Tilemap m_tilemapFront;

        private void Start()
        {
        }

        private void LateUpdate()
        {
            DeleteOutsideTiles();
        }

        // CopyTilesImmediate
        public void CopyTilesImmediate(Tilemap sourceTilemap, Vector3Int destPos, bool flip)
        {
            Dictionary<Vector3Int, TileBase> tiles = new Dictionary<Vector3Int, TileBase>();

            foreach (var position in sourceTilemap.cellBounds.allPositionsWithin)
            {
                if (sourceTilemap.HasTile(position))
                {
                    Vector3Int p = position;
                    if (flip)
                    {
                        p.x *= -1;
                    }
                    tiles.Add(destPos + p, sourceTilemap.GetTile(position));
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
                case "BackgroundShadeTile":
                    m_tilemapBackgroundShade.SetTiles(tiles.Keys.ToArray(), tiles.Values.ToArray());
                    break;
                case "DecorationTile":
                    m_tilemapFront.SetTiles(tiles.Keys.ToArray(), tiles.Values.ToArray());
                    break;
                case "Scroll_BackgroundTile":
                    m_tilemapBackgroundAdd.SetTiles(tiles.Keys.ToArray(), tiles.Values.ToArray());
                    break;
                default:
                    Debug.LogWarningFormat("tilemap has unexpected sorting layer: {0}", tr.sortingLayerName);
                    break;
            }
        }

        private void DeleteOutsideTiles()
        {
            // 画面外のタイルを削除する
            Tilemap[] tilemapArray = { m_tilemapObstacles, m_tilemapBackground, m_tilemapBackgroundDeco, m_tilemapBackgroundAdd,m_tilemapBackgroundShade,m_tilemapFront };

            foreach (Tilemap tilemap in tilemapArray)
            {
                foreach (Vector3Int position in tilemap.cellBounds.allPositionsWithin)
                {
                    if (tilemap.CellToWorld(position).y > GameController.Instance.WorldBoundary.center.y)
                    {
                        // Boundary 中心より上部のタイルは処理の対象外
                        continue;
                    }

                    if (tilemap.HasTile(position))
                    {
                        Bounds tileBounds = new Bounds(tilemap.GetCellCenterWorld(position), tilemap.cellSize);
                        if (!GameController.Instance.WorldBoundary.Intersects(tileBounds))
                        {
                            tilemap.SetTile(position, null);
                        }
                    }
                }
            }
        }

        // 大きめの敵がテレポートするのに十分な空間を探し、その位置を返す
        public Vector3[] QueryPositionsForTeleportation()
        {
            m_tilemapObstacles.CompressBounds();

            List<Vector3> results = new List<Vector3>();

            // タイルが配置されていない 3x3 の領域を見つける
            foreach (Vector3Int position in m_tilemapObstacles.cellBounds.allPositionsWithin)
            {
                if (m_tilemapObstacles.HasTile(position))
                {
                    continue;
                }

                Vector3Int[] aroundPositions = {
                    new Vector3Int(position.x - 1, position.y - 1, position.z),
                    new Vector3Int(position.x,     position.y - 1, position.z),
                    new Vector3Int(position.x + 1, position.y - 1, position.z),
                    new Vector3Int(position.x - 1, position.y,     position.z),
                    new Vector3Int(position.x + 1, position.y,     position.z),
                    new Vector3Int(position.x - 1, position.y + 1, position.z),
                    new Vector3Int(position.x,     position.y + 1, position.z),
                    new Vector3Int(position.x + 1, position.y + 1, position.z),
                };

                if (aroundPositions.All(pos => !m_tilemapObstacles.HasTile(pos)))
                {
                    results.Add(m_tilemapObstacles.CellToLocal(position));
                }
            }

            // 地面との距離が一定以上なら除外する
            float rayLength = 2;
            LayerMask collisionMask = LayerMask.GetMask("Obstacle");
            results.RemoveAll(position => {
                RaycastHit2D hit = Physics2D.Raycast(position, Vector2.down, rayLength, collisionMask);
                return !(bool)hit;
            });

            return results.ToArray();
        }

        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                Vector3[] positions = QueryPositionsForTeleportation();

                foreach (Vector3 position in positions)
                {
                    Gizmos.color = new Color(1, 1, 0, .2f);
                    Gizmos.DrawCube(position, Vector3.one * 3);
                }
            }
        }
    }
}
