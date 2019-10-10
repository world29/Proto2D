using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System.Linq;

namespace Proto2D
{
    public class ProceduralDungeonGeneratorWindow : EditorWindow
    {
        List<Bounds> m_boundsList;

        // 部屋を生成する空間の半径
        float radius = 150;

        // 部屋の幅の平均値
        float roomWidthMean = 50;
        // 部屋の高さの平均値
        float roomHeightMean = 50;
        // 部屋の広さの分散
        float roomSigma = 1;
        // メインの部屋の広さ (平均値からの倍率)
        float thresholdScalingForMainRoom = 1.25f;

        // 乱数生成器
        private RandomBoxMuller m_random;

        private TileBase m_tile;

        private Vector2Int m_tilemapSize;

        [MenuItem("Window/ProceduralDungeonGenerator")]
        static void Open()
        {
            EditorWindow.GetWindow<ProceduralDungeonGeneratorWindow>("ProceduralDungeonGenerator");
        }

        void OnGenerate()
        {
            MapGenerator gen = new MapGenerator();
            Vector2 roomSize = new Vector2(roomWidthMean, roomHeightMean);
            m_boundsList = gen.Generate(radius, roomSize, roomSigma, roomSize * thresholdScalingForMainRoom);
        }

        void OnRenderMap()
        {
            // シーンにあらかじめ配置された Tilemap を取得
            Tilemap tilemap = FindObjectOfType<Tilemap>();
            if (tilemap && m_tile)
            {
                HashSet<Vector3Int> map = new HashSet<Vector3Int>();
                for (int y = -m_tilemapSize.y / 2; y < m_tilemapSize.y / 2; y++)
                {
                    for (int x = -m_tilemapSize.x / 2; x < m_tilemapSize.x / 2; x++)
                    {
                        // 境界ボックスと衝突しないタイルを追加する
                        Vector3Int position = new Vector3Int(x, y, 0);
                        Bounds cellBounds = new Bounds(tilemap.GetCellCenterWorld(position), tilemap.cellSize);
                        if (m_boundsList.FindIndex(bounds => bounds.Intersects(cellBounds)) < 0)
                        {
                            map.Add(position);
                        }
                    }
                }

                RenderMap(map, tilemap, m_tile);
            }
        }

        public static void RenderMap(HashSet<Vector3Int> map, Tilemap tilemap, TileBase tile)
        {
            //マップをクリアする（重複しないようにする）
            tilemap.ClearAllTiles();

            foreach(Vector3Int position in map)
            {
                tilemap.SetTile(position, tile);
            }
        }

        private void Update()
        {
            Repaint();
        }

        void OnGUI()
        {
            radius = EditorGUILayout.Slider("radius", radius, 1, 50.0f);
            roomWidthMean = EditorGUILayout.Slider("width", roomWidthMean, 1, 10.0f);
            roomHeightMean = EditorGUILayout.Slider("height", roomHeightMean, 1, 10.0f);
            roomSigma = EditorGUILayout.Slider("sigma", roomSigma, 1, 10.0f);
            
            if (GUILayout.RepeatButton("Generate"))
                OnGenerate();

            m_tile = EditorGUILayout.ObjectField("Tile", m_tile, typeof(TileBase), false) as TileBase;
            m_tilemapSize.x = EditorGUILayout.IntSlider("tilemap width", m_tilemapSize.x, 20, 200);
            m_tilemapSize.y = EditorGUILayout.IntSlider("tilemap height", m_tilemapSize.y, 20, 500);

            if (GUILayout.Button("Render Map"))
                OnRenderMap();
        }
    }
}
