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
                case "BackgroundTile2":
                    m_tilemapBackgroundAdd.SetTiles(tiles.Keys.ToArray(), tiles.Values.ToArray());
                    break;
                default:
                    Debug.Assert(false, "tilemap has unexpected sorting layer");
                    break;
            }
        }

        private void DeleteOutsideTiles()
        {
            // 画面外のタイルを削除する
            Tilemap[] tilemapArray = { m_tilemapObstacles, m_tilemapBackground, m_tilemapBackgroundDeco, m_tilemapBackgroundAdd };

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
    }
}
