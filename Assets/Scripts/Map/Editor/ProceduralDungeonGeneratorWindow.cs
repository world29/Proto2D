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

        private string m_parameters_path = "Assets/Sandbox/MapGenerationParameters.asset";
        private MapGenerationParameters m_parameters;
        private TileBase m_tile;
        private Vector2Int m_tilemapSize;

        [MenuItem("Window/ProceduralDungeonGenerator")]
        static void Open()
        {
            EditorWindow window = EditorWindow.GetWindow<ProceduralDungeonGeneratorWindow>("ProceduralDungeonGenerator");
            window.minSize = new Vector2(300f, 300f);
        }

        void OnGenerateRooms()
        {
            //Debug
            {
                Tilemap tilemap = FindObjectOfType<Tilemap>();
                if (tilemap)
                {
                    tilemap.ClearAllTiles();
                }
            }

            MapGenerator gen = new MapGenerator();
            m_boundsList = gen.Generate(m_parameters);

            // スタート部屋の生成

            // プリセット部屋の生成
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

        void LoadOrCreateParameters()
        {
            m_parameters = AssetDatabase.LoadAssetAtPath<MapGenerationParameters>(m_parameters_path);

            if (m_parameters == null)
            {
                // ロードしてnullだったら存在しないので生成
                m_parameters = ScriptableObject.CreateInstance<MapGenerationParameters>();
                AssetDatabase.CreateAsset(m_parameters, m_parameters_path);
            }
        }

        void SaveParameters()
        {
            EditorUtility.SetDirty(m_parameters);
        }

        private void Update()
        {
            Repaint();
        }

        void OnGUI()
        {
            if (GUILayout.Button("Load or Create Parameters"))
                LoadOrCreateParameters();

            if (m_parameters)
            {
                m_parameters.seed = EditorGUILayout.IntSlider("seed", m_parameters.seed, 1, 10000);
                m_parameters.simulationSteps = EditorGUILayout.IntSlider("simulation steps", m_parameters.simulationSteps, 1, 5000);

                m_parameters.roomGenerationCount = EditorGUILayout.IntSlider("room gen count", m_parameters.roomGenerationCount, 1, 1000);
                m_parameters.roomGenerationAreaRadius = EditorGUILayout.Slider("room gen area radius", m_parameters.roomGenerationAreaRadius, 1, 50.0f);

                m_parameters.roomGenerationSizeMeanX = EditorGUILayout.Slider("room size mean x", m_parameters.roomGenerationSizeMeanX, 1, 10.0f);
                m_parameters.roomGenerationSizeMeanY = EditorGUILayout.Slider("room size mean y", m_parameters.roomGenerationSizeMeanY, 1, 10.0f);
                m_parameters.roomGenerationSizeSigma = EditorGUILayout.Slider("room size sigma", m_parameters.roomGenerationSizeSigma, 1, 10.0f);

                m_parameters.mainRoomThresholdX = EditorGUILayout.Slider("room selection threshold x", m_parameters.mainRoomThresholdX, 1, 10.0f);
                m_parameters.mainRoomThresholdY = EditorGUILayout.Slider("room selection threshold y", m_parameters.mainRoomThresholdY, 1, 10.0f);
                m_parameters.hallwayWidth = EditorGUILayout.Slider("hallways width", m_parameters.hallwayWidth, 1, 10.0f);

                if (GUILayout.Button("Save Parameters"))
                    SaveParameters();

                if (GUILayout.Button("Generate Rooms"))
                    OnGenerateRooms();
            }

            m_tile = EditorGUILayout.ObjectField("Tile", m_tile, typeof(TileBase), false) as TileBase;
            m_tilemapSize.x = EditorGUILayout.IntSlider("tilemap width", m_tilemapSize.x, 20, 200);
            m_tilemapSize.y = EditorGUILayout.IntSlider("tilemap height", m_tilemapSize.y, 20, 500);

            if (GUILayout.Button("Render Map"))
                OnRenderMap();
        }
    }
}
