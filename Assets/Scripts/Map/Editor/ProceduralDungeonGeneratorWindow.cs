using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System.Linq;

namespace Proto2D
{
    // シーンビューでデバッグ表示するためのコンポーネント
    public class RoomDebug : MonoBehaviour
    {
        public Vector3 m_center;
        public Vector3 m_size;

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0, 1, 1, .3f);
            Gizmos.DrawCube(m_center, m_size);
        }
    }

    public class ProceduralDungeonGeneratorWindow : EditorWindow
    {
        List<Bounds> m_boundsList;

        private string m_parameters_path = "Assets/Resources/MapGenerationParameters.asset";
        private MapGenerationParameters m_parameters;
        private TileBase m_tile;
        private Vector2Int m_tilemapSize;

        private bool m_randomSeed = true;
        private GameObject m_startRoomPrefab;
        private float m_mainHallwayWidth;

        [MenuItem("Window/ProceduralDungeonGenerator")]
        static void Open()
        {
            EditorWindow window = EditorWindow.GetWindow<ProceduralDungeonGeneratorWindow>("ProceduralDungeonGenerator");
            window.minSize = new Vector2(300f, 300f);
        }

        void OnGenerateRooms()
        {
            {
                // タイルマップ初期化
                Tilemap tilemap = FindObjectOfType<Tilemap>();
                if (tilemap)
                {
                    tilemap.ClearAllTiles();
                }

                // デバッグ表示用オブジェクトを削除
                foreach(var room in GameObject.FindObjectsOfType<RoomDebug>())
                {
                    GameObject.DestroyImmediate(room.gameObject);
                }
            }

            // シード値を乱数で設定
            if (m_randomSeed)
            {
                m_parameters.seed = Random.Range(1, 10000);
            }

            MapGenerator gen = new MapGenerator();
            m_boundsList = gen.Generate(m_parameters, Vector3.zero);

            // デバッグ表示用オブジェクトの作成
            m_boundsList.ForEach(bounds =>
            {
                GameObject obj = new GameObject("RoomDebug");
                RoomDebug room = obj.AddComponent<RoomDebug>();
                room.m_center = bounds.center;
                room.m_size = bounds.size;
            });
        }

        void OnRenderMap()
        {
            // シーンにあらかじめ配置された Tilemap を取得
            Tilemap tilemap = FindObjectOfType<Tilemap>();
            if (tilemap && m_tile)
            {
                // タイルマップの下端にプリセット部屋 (PlayerStart) を生成
                if (m_startRoomPrefab)
                {
                    RoomController rc = m_startRoomPrefab.GetComponent<RoomController>();
                    float bottomY = -m_tilemapSize.y / 2;
                    Vector3 spawnPosition = new Vector3(0, bottomY + rc.OriginToCenter.y, 0);
                    GameObject.Instantiate(m_startRoomPrefab, spawnPosition, Quaternion.identity);

                    //TODO: タイルマップの転写

                    // 一番下の部屋からプリセット部屋へつづく通路を生成
                    Bounds bottomRoom = m_boundsList.Aggregate((bottom, cur) => bottom.center.y < cur.center.y ? bottom : cur);
                    Bounds startHallway = new Bounds();
                    startHallway.SetMinMax(new Vector2(-m_mainHallwayWidth / 2, spawnPosition.y), new Vector2(m_mainHallwayWidth / 2, bottomRoom.center.y));
                    m_boundsList.Add(startHallway);
                }

                //TODO: タイルマップの上端にプリセット部屋を生成

                // タイルマップ更新
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
                m_parameters.roomGenerationAreaWidth = EditorGUILayout.Slider("room gen area width", m_parameters.roomGenerationAreaWidth, 1, 100.0f);
                m_parameters.roomGenerationAreaHeight = EditorGUILayout.Slider("room gen area height", m_parameters.roomGenerationAreaHeight, 1, 100.0f);

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

            m_randomSeed = EditorGUILayout.Toggle("random seed", m_randomSeed);
            GameObject obj = EditorGUILayout.ObjectField("Start Room", m_startRoomPrefab, typeof(GameObject), false) as GameObject;
            if (obj && obj.GetComponent<RoomController>())
            {
                m_startRoomPrefab = obj;
            }
            else
            {
                m_startRoomPrefab = null;
            }
            m_mainHallwayWidth = EditorGUILayout.Slider("main hallways width", m_mainHallwayWidth, 1, 10.0f);

            if (GUILayout.Button("Render Map"))
                OnRenderMap();
        }
    }
}
