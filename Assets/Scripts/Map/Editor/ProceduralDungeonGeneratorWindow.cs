using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System.Linq;

namespace Proto2D
{
    public class PhysicsBox
    {
        public Rect m_rect;
        public GameObject m_object;
        public Room m_room;

        public PhysicsBox(Rect rect)
        {
            m_rect = rect;

            Init();
        }

        public void Init()
        {
            m_object = new GameObject("PhysicsBox");
            m_object.transform.position = new Vector3(m_rect.center.x, m_rect.center.y, 0);

            Rigidbody2D rigidbody = m_object.AddComponent<Rigidbody2D>();
            rigidbody.gravityScale = 0;
            rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;

            BoxCollider2D collider = m_object.AddComponent<BoxCollider2D>();
            collider.size = new Vector3(m_rect.size.x, m_rect.size.y, 1);

            m_room = m_object.AddComponent<Room>();
        }

        public void Destroy()
        {
            GameObject.DestroyImmediate(m_object);
        }
    }

    public class ProceduralDungeonGeneratorWindow : EditorWindow
    {
        List<PhysicsBox> m_rooms = new List<PhysicsBox>();

        List<DelaunayTriangulation.Triangle> m_triangles;

        List<DelaunayTriangulation.Edge> m_spanning_tree;

        List<Bounds> m_boundsList = new List<Bounds>();

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

        void OnReset()
        {
            m_random = new RandomBoxMuller();

            m_rooms.Clear();
            m_boundsList.Clear();

            Rigidbody2D[] simulatedBodies = GameObject.FindObjectsOfType<Rigidbody2D>();
            foreach (var rb in simulatedBodies)
            {
                GameObject.DestroyImmediate(rb.gameObject);
            }
        }

        void OnGenerate()
        {
            if (m_random == null)
            {
                OnReset();
            }

            // 幅と高さを持つ部屋を円の中にランダムに配置
            Vector2 center = m_random.Source.insideUnitCircle * radius;

            // 各部屋のサイズを指定するのに正規分布を使用する
            Vector2 size;
            size.x = m_random.Next(roomWidthMean, roomSigma, true);
            size.y = m_random.Next(roomHeightMean, roomSigma, false);

            m_rooms.Add(new PhysicsBox(new Rect(center, size)));
        }

        void OnSimulate(int stepCount = 1000)
        {
            Rigidbody2D[] simulatedBodies = GameObject.FindObjectsOfType<Rigidbody2D>();
            foreach (var rb in simulatedBodies)
            {
                rb.AddForce(Vector2.up);
            }

            Physics2D.autoSimulation = false;
            for (int i = 0; i < stepCount; i++)
            {
                Physics2D.Simulate(Time.fixedDeltaTime);

                if (simulatedBodies.All(item => item.IsSleeping()))
                {
                    Debug.Log(i);
                    break;
                }
            }
            Physics2D.autoSimulation = true;
        }

        void OnSelect()
        {
            Vector2 minSize = new Vector2(roomWidthMean, roomHeightMean) * thresholdScalingForMainRoom;
            List<PhysicsBox> selection = m_rooms.Where(item => item.m_rect.size.x > minSize.x && item.m_rect.size.y > minSize.y).ToList();
            Debug.Log(selection.Count);
            selection.ForEach(item => item.m_room.selected = true);

            // メインの部屋を境界ボックスリストに追加
            selection.ForEach(item => {
                m_boundsList.Add(new Bounds(item.m_object.transform.position, item.m_rect.size));
            });
        }

        void OnTriangulate()
        {
            //List<Vector2> points = m_rooms.Select(item => (Vector2)item.m_object.transform.position).ToList();
            List<Vector2> points = m_rooms
                .Where(item => item.m_room.selected)
                .Select(item => (Vector2)item.m_object.transform.position)
                .ToList();

            foreach(Vector2 point in points)
            {
                Debug.DrawLine(point, point + Vector2.one * .1f, Color.red, 1);
            }

            m_triangles = DelaunayTriangulation.Calculate(points);

            //DrawTriangles(m_triangles);
        }

        void OnCalculateSpanningTree()
        {
            List<DelaunayTriangulation.Edge> edges = m_triangles.Aggregate(new List<DelaunayTriangulation.Edge>(), (sum, next) => sum.Concat(next.Edges).ToList());
            m_spanning_tree = KruskalSpanningTree.Calculate(edges);

            m_spanning_tree.ForEach(edge =>
            {
                Debug.DrawLine(edge.start, edge.end, Color.red, 1);
            });
        }

        void OnGenerateHallways()
        {
            // 部屋をつなぐ廊下を作る
            var mainRooms = m_rooms.Where(item => item.m_room.selected).ToList();
            var hallways = m_rooms.Where(item => !mainRooms.Contains(item)).ToList();

            // 線分を表す Vector4
            // 両端の点を (a, b) とした場合
            // (x, y, z, w) = (a.x, a.y, b.x, b.y)
            List<Vector4> lines = new List<Vector4>();

            // 全域木に含まれる辺に関して、
            // 辺の両端の部屋をつなぐラインを引く
            // ラインは水平あるいは垂直方向にのみ伸びる。
            // 部屋どうしの垂直/水平方向のズレが小さければ一本でよいが、そうでなければ二本。
            m_spanning_tree.ForEach(edge =>
            {
                var A = mainRooms.Find(item => item.m_object.transform.position.Equals(edge.start));
                var B = mainRooms.Find(item => item.m_object.transform.position.Equals(edge.end));

                Rect rect_A = new Rect((Vector2)A.m_object.transform.position - A.m_rect.size/ 2, A.m_rect.size);
                Rect rect_B = new Rect((Vector2)B.m_object.transform.position - B.m_rect.size / 2, B.m_rect.size);

                // 中間点が部屋の境界を超えるならラインが二本必要
                var midpoint = (rect_A.center + rect_B.center) / 2;

                // 垂直に並んでいる
                if (midpoint.x > rect_A.xMin && midpoint.x < rect_A.xMax)
                {
                    lines.Add(new Vector4(midpoint.x, rect_A.center.y, midpoint.x, rect_B.center.y));
                }
                // 水平に並んでいる
                else if (midpoint.y > rect_A.yMin && midpoint.y < rect_A.yMax)
                {
                    lines.Add(new Vector4(rect_A.center.x, midpoint.y, rect_B.center.x, midpoint.y));
                }
                else
                {
                    lines.Add(new Vector4(rect_A.center.x, rect_A.center.y, rect_A.center.x, rect_B.center.y));
                    lines.Add(new Vector4(rect_B.center.x, rect_B.center.y, rect_A.center.x, rect_B.center.y));
                }
            });

            lines.ForEach(line =>
            {
                Debug.DrawLine(new Vector2(line.x, line.y), new Vector2(line.z, line.w), Color.blue, 1);
            });

            var lineBoundsList = lines.Select(line => {
                Vector2 a = new Vector2(line.x, line.y);
                Vector2 b = new Vector2(line.z, line.w);

                //TODO: 廊下の最小幅を設定する
                float minWayWidth = 3;

                Vector2 size = b - a;
                size.x = Mathf.Max(minWayWidth, Mathf.Abs(size.x));
                size.y = Mathf.Max(minWayWidth, Mathf.Abs(size.y));

                return new Bounds((a + b) / 2, size);
            }).ToList();

            // メインでない部屋のうち、ラインと交差する部屋を廊下の一部とみなす
            // それ以外の部屋は削除する
            var roomsToDelete = hallways.FindAll(item => {
                Bounds hallwaysBounds = new Bounds(item.m_object.transform.position, item.m_rect.size);
                return lineBoundsList.FindIndex(bounds => bounds.Intersects(hallwaysBounds)) == -1;
            });
            roomsToDelete.ForEach(item => {
                item.Destroy();
                hallways.Remove(item);
            });

            // 廊下を表すラインとサブの部屋を境界ボックスリストに追加
            m_boundsList.AddRange(lineBoundsList);
            hallways.ForEach(way => {
                m_boundsList.Add(new Bounds(way.m_object.transform.position, way.m_rect.size));
            });
        }

        void OnRenderMap()
        {
            int[,] map = GenerateArray(m_tilemapSize.x, m_tilemapSize.y, false);

            // シーンにあらかじめ配置された Tilemap を取得
            Tilemap tilemap = FindObjectOfType<Tilemap>();
            if (tilemap && m_tile)
            {
                RenderMap(map, tilemap, m_tile);

                // 境界ボックスと衝突するタイルを消す
                foreach (var position in tilemap.cellBounds.allPositionsWithin)
                {
                    if (tilemap.HasTile(position))
                    {
                        Bounds cellBounds = new Bounds(tilemap.GetCellCenterWorld(position), tilemap.cellSize);

                        if (m_boundsList.FindIndex(bounds => bounds.Intersects(cellBounds)) >= 0)
                        {
                            map[position.x, position.y] = 0;
                        }
                    }
                }

                // map を更新
                UpdateMap(map, tilemap);
            }
        }

        public static int[,] GenerateArray(int width, int height, bool empty)
        {
            int[,] map = new int[width, height];
            for (int x = 0; x < map.GetUpperBound(0); x++)
            {
                for (int y = 0; y < map.GetUpperBound(1); y++)
                {
                    if (empty)
                    {
                        map[x, y] = 0;
                    }
                    else
                    {
                        map[x, y] = 1;
                    }
                }
            }
            return map;
        }

        public static void RenderMap(int[,] map, Tilemap tilemap, TileBase tile)
        {
            //マップをクリアする（重複しないようにする）
            tilemap.ClearAllTiles();
            //マップの幅の分、周回する
            for (int x = 0; x < map.GetUpperBound(0); x++)
            {
                //マップの高さの分、周回する
                for (int y = 0; y < map.GetUpperBound(1); y++)
                {
                    // 1 = タイルあり、0 = タイルなし
                    if (map[x, y] == 1)
                    {
                        tilemap.SetTile(new Vector3Int(x, y, 0), tile);
                    }
                }
            }
        }

        public static void UpdateMap(int[,] map, Tilemap tilemap) //マップとタイルマップを取得し、null タイルを必要箇所に設定する
        {
            for (int x = 0; x < map.GetUpperBound(0); x++)
            {
                for (int y = 0; y < map.GetUpperBound(1); y++)
                {
                    //再レンダリングではなく、マップの更新のみを行う
                    //これは、それぞれのタイル（および衝突データ）を再描画するのに比べて
                    //タイルを null に更新するほうが使用リソースが少なくて済むためです。
                    if (map[x, y] == 0)
                    {
                        tilemap.SetTile(new Vector3Int(x, y, 0), null);
                    }
                }
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

            if (GUILayout.Button("Reset"))
                OnReset();

            if (GUILayout.RepeatButton("Generate"))
                OnGenerate();

            if (GUILayout.Button("Simulate"))
                OnSimulate();

            if (GUILayout.Button("Select Rooms"))
                OnSelect();

            if (GUILayout.Button("Triangulate"))
                OnTriangulate();

            if (GUILayout.Button("Calculate Spanning Tree"))
                OnCalculateSpanningTree();

            if (GUILayout.Button("Generate Hallways"))
                OnGenerateHallways();

            m_tile = EditorGUILayout.ObjectField("Tile", m_tile, typeof(TileBase), false) as TileBase;
            m_tilemapSize.x = EditorGUILayout.IntSlider("tilemap width", m_tilemapSize.x, 20, 200);
            m_tilemapSize.y = EditorGUILayout.IntSlider("tilemap height", m_tilemapSize.y, 20, 500);

            if (GUILayout.Button("Render Map"))
                OnRenderMap();
        }

        public void DrawTriangles(List<DelaunayTriangulation.Triangle> triangles)
        {
            foreach(var tri in triangles)
            {
                tri.Draw(Color.cyan, 1);
            }
        }
    }
}
