using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using System.Linq;
using System.Text.RegularExpressions;

namespace Assets.NewData.Scripts
{
    public class CopyRoomToScene : EditorWindow
    {
        private Tilemap _destTilemap;

        [SerializeField]
        private Tilemap[] _destTilemapsBackground;

        private Proto2D.RoomController _sourceRoomPrefab;
        private TileBase _tile;

        private Transform _propsRoot;
        private Vector3 _propsOffset;

        private void DoCopyTiles()
        {
            if (_sourceRoomPrefab == null)
            {
                return;
            }

            Dictionary<string, Tilemap> sortingLayerToTilemap = new Dictionary<string, Tilemap>();

            // ソーティングレイヤーごとのタイルマップを格納
            foreach (var tilemap in _sourceRoomPrefab.GetComponentsInChildren<Tilemap>())
            {
                var tilemapRenderer = tilemap.GetComponent<TilemapRenderer>();

                Debug.Assert(!sortingLayerToTilemap.ContainsKey(tilemapRenderer.sortingLayerName), "tilemap conflicted.");

                sortingLayerToTilemap[tilemapRenderer.sortingLayerName] = tilemap;
            }

            Vector3Int offset;

            // 基準となるタイルマップを転写する
            {
                var sourceTilemapObstacle = sortingLayerToTilemap["ObstacleTile"];
                sortingLayerToTilemap.Remove("ObstacleTile");

                sourceTilemapObstacle.CompressBounds();
                _destTilemap.CompressBounds();

                // コピー先タイルマップの上端の座標
                // CompressTilemapBounds を行わないと正しい境界が得られないので注意。
                Debug.Log($"Range(x): ({_destTilemap.cellBounds.xMin},{_destTilemap.cellBounds.xMax})");
                Debug.Log($"Range(y): ({_destTilemap.cellBounds.yMin},{_destTilemap.cellBounds.yMax})");

                Debug.Log($"Origin: {sourceTilemapObstacle.origin.y}");

                offset = new Vector3Int(0, _destTilemap.cellBounds.yMax - sourceTilemapObstacle.origin.y, 0);

                CopyTiles(_destTilemap, offset, sourceTilemapObstacle, _tile);

                // 背景小物をコピーするためのオフセットを計算する
                _propsOffset = new Vector3(offset.x * sourceTilemapObstacle.cellSize.x, offset.y * sourceTilemapObstacle.cellSize.y, 0);
            }

            // 背景タイルマップを転写する
            foreach (KeyValuePair<string, Tilemap> entry in sortingLayerToTilemap)
            {
                Tilemap destTilemap = _destTilemapsBackground
                    .FirstOrDefault(x => x.GetComponent<TilemapRenderer>().sortingLayerName == entry.Key);

                if (destTilemap)
                {
                    CopyTiles(destTilemap, offset, entry.Value);
                }
                else
                {
                    Debug.LogWarning($"destination Tilemap is not exist. SortingLayer: {entry.Key}");
                }
            }
        }

        private void DoCopyProps()
        {
            for (int i = 0; i < _sourceRoomPrefab.transform.childCount; i++)
            {
                GameObject obj = _sourceRoomPrefab.transform.GetChild(i).gameObject;

                Regex regex = new Regex("_Deco");
                if (regex.IsMatch(obj.name))
                {
                    var go = Instantiate(obj, obj.transform.localPosition + _propsOffset, obj.transform.rotation, _propsRoot);
                    Undo.RegisterCreatedObjectUndo(go, "Instantiate Prop Object");
                }
            }
        }

        static void CopyTiles(Tilemap destTilemap, Vector3Int offset, Tilemap sourceTilemap, TileBase _tileToReplace = null)
        {
            Debug.Log($"Copy {sourceTilemap} to {destTilemap.name}");

            Dictionary<Vector3Int, TileBase> tiles = new Dictionary<Vector3Int, TileBase>();

            foreach (var sourcePos in sourceTilemap.cellBounds.allPositionsWithin)
            {
                if (sourceTilemap.HasTile(sourcePos))
                {
                    Vector3Int destPos = sourcePos + offset;

                    if (_tileToReplace != null)
                    {
                        tiles.Add(destPos, _tileToReplace);
                    }
                    else
                    {
                        tiles.Add(destPos, sourceTilemap.GetTile(sourcePos));
                    }
                }
            }

            // タイルのコピーを実行
            Undo.RecordObject(destTilemap, "SetTiles");
            destTilemap.SetTiles(tiles.Keys.ToArray(), tiles.Values.ToArray());
        }

        private void Update()
        {
            Repaint();
        }

        void OnGUI()
        {
            _destTilemap = EditorGUILayout.ObjectField("Tilemap", _destTilemap, typeof(Tilemap), true) as Tilemap;

            {
                ScriptableObject target = this;
                SerializedObject so = new SerializedObject(target);
                SerializedProperty tilemapsProperty = so.FindProperty("_destTilemapsBackground");
                if (tilemapsProperty != null)
                {
                    EditorGUILayout.PropertyField(tilemapsProperty, true);
                    so.ApplyModifiedProperties();
                }
            }

            _sourceRoomPrefab = EditorGUILayout.ObjectField("Room Prefab", _sourceRoomPrefab, typeof(Proto2D.RoomController), true) as Proto2D.RoomController;

            _tile = EditorGUILayout.ObjectField("Tile", _tile, typeof(TileBase), false) as TileBase;

            if (GUILayout.Button("Copy Tiles"))
                DoCopyTiles();

            EditorGUILayout.Separator();

            _propsRoot = EditorGUILayout.ObjectField("Props Root", _propsRoot, typeof(Transform), true) as Transform;
            _propsOffset = EditorGUILayout.Vector3Field("Props Offset", _propsOffset);

            if (GUILayout.Button("Copy Props"))
                DoCopyProps();
        }

        [MenuItem("Window/CopyRoomToScene")]
        static void Open()
        {
            EditorWindow window = EditorWindow.GetWindow<CopyRoomToScene>("CopyRoomToScene");
            window.minSize = new Vector2(300f, 300f);
        }
    }
}
