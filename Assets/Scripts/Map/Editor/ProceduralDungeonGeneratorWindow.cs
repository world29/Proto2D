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

        private const int kRandomSeedMax = 10000;

        MapGenerator.Parameters parameters = new MapGenerator.Parameters();

        private TileBase m_tile;

        private Vector2Int m_tilemapSize;

        [MenuItem("Window/ProceduralDungeonGenerator")]
        static void Open()
        {
            EditorWindow.GetWindow<ProceduralDungeonGeneratorWindow>("ProceduralDungeonGenerator");
        }

        void OnGenerate()
        {
            //Debug
            {
                Tilemap tilemap = FindObjectOfType<Tilemap>();
                if (tilemap)
                {
                    tilemap.ClearAllTiles();
                }

                var objects = GameObject.FindObjectsOfType<MapGenerationSeed>();
                foreach(var obj in objects)
                {
                    GameObject.DestroyImmediate(obj.gameObject);
                }
            }

            MapGenerator gen = new MapGenerator();
            m_boundsList = gen.Generate(parameters);

            m_boundsList.ForEach(bounds => {
                Debug.DrawLine(bounds.min, bounds.max, Color.red, 1);
            });
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
            parameters.seed = EditorGUILayout.IntSlider("seed", parameters.seed, 1, kRandomSeedMax);
            parameters.simulationSteps = EditorGUILayout.IntSlider("simulation steps", parameters.simulationSteps, 1, 5000);

            parameters.roomGenerationCount = EditorGUILayout.IntSlider("room gen count", parameters.roomGenerationCount, 1, 1000);
            parameters.roomGenerationAreaRadius = EditorGUILayout.Slider("room gen area radius", parameters.roomGenerationAreaRadius, 1, 50.0f);

            parameters.roomGenerationSizeMeanX = EditorGUILayout.Slider("room size mean x", parameters.roomGenerationSizeMeanX, 1, 10.0f);
            parameters.roomGenerationSizeMeanY = EditorGUILayout.Slider("room size mean y", parameters.roomGenerationSizeMeanY, 1, 10.0f);
            parameters.roomGenerationSizeSigma = EditorGUILayout.Slider("room size sigma", parameters.roomGenerationSizeSigma, 1, 10.0f);

            parameters.mainRoomThresholdX = EditorGUILayout.Slider("room selection threshold x", parameters.mainRoomThresholdX, 1, 10.0f);
            parameters.mainRoomThresholdY = EditorGUILayout.Slider("room selection threshold y", parameters.mainRoomThresholdY, 1, 10.0f);
            parameters.hallwayWidth = EditorGUILayout.Slider("hallways width", parameters.hallwayWidth, 1, 10.0f);

            if (GUILayout.Button("Generate"))
                OnGenerate();

            m_tile = EditorGUILayout.ObjectField("Tile", m_tile, typeof(TileBase), false) as TileBase;
            m_tilemapSize.x = EditorGUILayout.IntSlider("tilemap width", m_tilemapSize.x, 20, 200);
            m_tilemapSize.y = EditorGUILayout.IntSlider("tilemap height", m_tilemapSize.y, 20, 500);

            if (GUILayout.Button("Render Map"))
                OnRenderMap();

            if (GUILayout.Button("Simulation"))
            {
                var targets = GameObject.FindObjectsOfType<Rigidbody2D>();
                foreach (var rb in targets)
                {
                    float sheta = Random.Range(0, 2 * Mathf.PI);
                    rb.AddForce(new Vector2(Mathf.Cos(sheta), Mathf.Sin(sheta)));
                }

                Physics2D.autoSimulation = false;
                for (int i = 0; i < parameters.simulationSteps; i++)
                {
                    Physics2D.Simulate(Time.fixedDeltaTime);

                    if (targets.All(item => item.IsSleeping()))
                    {
                        break;
                    }
                }
                Physics2D.autoSimulation = true;
            }
        }
    }
}
