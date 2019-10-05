using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class RoomController : MonoBehaviour
{
    public Vector3Int Size { get { return primaryTilemap.size; } }
    public Vector3 CellSize { get { return primaryTilemap.cellSize; } }
    // タイルマップの左下端から (0,0) へのオフセット
    public Vector3 OriginToCenter { get { return -primaryTilemap.CellToLocal(primaryTilemap.origin); } }

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
