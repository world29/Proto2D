using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Proto2D
{
    public class MapGenerator
    {
        private RandomBoxMuller m_random;
        private List<GameObject> m_rooms;

        // [生成パラメータ]
        public List<Bounds> Generate(MapGenerationParameters parameters, Vector3 position)
        {
            // 乱数の初期化
            m_random = new RandomBoxMuller(parameters.seed);

            // 部屋生成
            Bounds generationArea = new Bounds(position, new Vector3(parameters.roomGenerationAreaWidth, parameters.roomGenerationAreaHeight));
            Bounds roomSpawnArea = generationArea;
            //HACK: 物理シミュレーションによって全体的に外側に膨らむので、半分のエリアに生成する。
            roomSpawnArea.Expand(.5f);
            Vector2 roomSizeMean = new Vector2(parameters.roomGenerationSizeMeanX, parameters.roomGenerationSizeMeanY);
            List<GameObject> rooms = SpawnRooms(
                parameters.roomGenerationCount,
                roomSpawnArea,
                roomSizeMean,
                parameters.roomGenerationSizeSigma);

            // 物理シミュレーション
            List<Rigidbody2D> rigidbodies = rooms.Select(item => item.GetComponent<Rigidbody2D>()).ToList();
            Simulation(rigidbodies, parameters.simulationSteps);

            // 境界ボックスに変換
            List<Bounds> boundsList = rooms.Select(item =>
            {
                Bounds bounds = new Bounds(item.transform.position, item.GetComponent<BoxCollider2D>().size);
                // GameObject はもう必要ない
                GameObject.DestroyImmediate(item);
                return bounds;
            }).ToList();

            // 部屋とそれ以外に選別
            Vector2 mainRoomSize = new Vector2(parameters.mainRoomThresholdX, parameters.mainRoomThresholdY);
            List<Bounds> mainRooms = SelectRooms(boundsList, generationArea, mainRoomSize);
            List<Bounds> otherRooms = boundsList.Except(mainRooms).ToList();

            // 部屋の配置が偏る場合があるので、位置を補正する
            Vector3 centerOfRooms = mainRooms.Select(bounds => bounds.center - position).Aggregate((sum, cur) => sum + cur);
            centerOfRooms /= mainRooms.Count;
            for (int i = 0; i < mainRooms.Count; i++)
            {
                Bounds bounds = mainRooms[i];
                mainRooms[i] = new Bounds(bounds.center - centerOfRooms, bounds.size);
            }
            for (int i = 0; i < otherRooms.Count; i++)
            {
                Bounds bounds = otherRooms[i];
                otherRooms[i] = new Bounds(bounds.center - centerOfRooms, bounds.size);
            }

            // 三角化
            List<Vector3> vertices = mainRooms.Select(item => item.center).ToList();
            List<Triangle> triangles = Delaunay.Triangulate(vertices);

#if UNITY_EDITOR
            triangles.ForEach(item => item.Draw(Color.cyan, 1));
#endif

            // 最小全域木
            List<Edge> edgesOfTriangles = triangles.Aggregate(new List<Edge>(), (sum, next) => sum.Concat(next.Edges).ToList());
            List<Edge> tree = Kruskal.CalculateSpanningTree(edgesOfTriangles);

            // 廊下を調整
            List<Bounds> hallways = CalculateHallways(mainRooms, tree, otherRooms, parameters.hallwayWidth);

            return mainRooms.Union(hallways).ToList();
        }

        public List<GameObject> SpawnRooms(int count, Bounds spawnArea, Vector2 sizeMean, float sigma)
        {
            List<GameObject> rooms = new List<GameObject>();

            for (int i = 0; i < count; i++)
            {
                // 幅と高さを持つ部屋を円の中にランダムに配置
                Vector2 pos = m_random.Source.insideUnitCircle;
                Vector3 center = new Vector3(spawnArea.center.x + pos.x * spawnArea.extents.x, spawnArea.center.y + pos.y * spawnArea.extents.y);

                // 各部屋のサイズを指定するのに正規分布を使用する
                Vector2 size;
                size.x = m_random.Next(sizeMean.x, sigma, true);
                size.y = m_random.Next(sizeMean.y, sigma, false);

                rooms.Add(createRoomObject(center, size));
            }

            return rooms;
        }

        public void Simulation(List<Rigidbody2D> targets, int step)
        {
            foreach (var rb in targets)
            {
                float sheta = Random.Range(0, 2 * Mathf.PI);
                rb.AddForce(new Vector2(Mathf.Cos(sheta), Mathf.Sin(sheta)));
            }

            //Physics2D.autoSimulation = false;
            for (int i = 0; i < step; i++)
            {
                Physics2D.Simulate(Time.fixedDeltaTime);

                if (targets.All(item => item.IsSleeping()))
                {
                    break;
                }
            }
            //Physics2D.autoSimulation = true;
        }

        public List<Bounds> SelectRooms(List<Bounds> seeds, Bounds roomArea, Vector2 roomSize)
        {
            // 指定エリア内にあり、指定サイズ以上の部屋を選択する
            return seeds
                .Where(bounds => roomArea.Contains(bounds.min) && roomArea.Contains(bounds.max))
                .Where(bounds => bounds.size.x >= roomSize.x && bounds.size.y >= roomSize.y).ToList();
        }

        public List<Bounds> CalculateHallways(List<Bounds> rooms, List<Edge> edges, List<Bounds> candidates, float hallwayWidth)
        {
            List<Bounds> hallwaysBoundsList = new List<Bounds>();

            // 線分を表す Vector4
            // 両端の点を (a, b) とした場合
            // (x, y, z, w) = (a.x, a.y, b.x, b.y)
            List<Vector4> lines = new List<Vector4>();

            // 全域木に含まれる辺に関して、
            // 辺の両端の部屋をつなぐラインを引く
            // ラインは水平あるいは垂直方向にのみ伸びる。
            // 部屋どうしの垂直/水平方向のズレが小さければ一本でよいが、そうでなければ二本。
            edges.ForEach(edge =>
            {
                var boundsA = rooms.Find(item => item.center.Equals(edge.start));
                var boundsB = rooms.Find(item => item.center.Equals(edge.end));

                // 境界ボックスが重なっているならラインは必要ない
                if (!boundsA.Intersects(boundsB))
                {
                    // 中間点が部屋の境界を超えるならラインが二本必要
                    var midpoint = (boundsA.center + boundsB.center) / 2;

                    // 垂直に並んでいる
                    if (midpoint.x > boundsA.min.x && midpoint.x < boundsA.max.x)
                    {
                        lines.Add(new Vector4(midpoint.x, boundsA.center.y, midpoint.x, boundsB.center.y));
                    }
                    // 水平に並んでいる
                    else if (midpoint.y > boundsA.min.y && midpoint.y < boundsA.max.y)
                    {
                        lines.Add(new Vector4(boundsA.center.x, midpoint.y, boundsB.center.x, midpoint.y));
                    }
                    else
                    {
                        lines.Add(new Vector4(boundsA.center.x, boundsA.center.y, boundsA.center.x, boundsB.center.y));
                        lines.Add(new Vector4(boundsB.center.x, boundsB.center.y, boundsA.center.x, boundsB.center.y));
                    }
                }
            });

            /*
            lines.ForEach(line =>
            {
                Debug.DrawLine(new Vector2(line.x, line.y), new Vector2(line.z, line.w), Color.blue, 1);
            });
            */

            // ラインを境界ボックスに変換
            lines.ForEach(line => {
                Vector2 a = new Vector2(line.x, line.y);
                Vector2 b = new Vector2(line.z, line.w);

                Vector2 size = b - a;
                size.x = Mathf.Max(hallwayWidth, Mathf.Abs(size.x));
                size.y = Mathf.Max(hallwayWidth, Mathf.Abs(size.y));

                hallwaysBoundsList.Add(new Bounds((a + b) / 2, size));
            });

            // ラインと交差する境界ボックスを廊下の一部とみなす
            hallwaysBoundsList.AddRange(candidates.FindAll(item => {
                return hallwaysBoundsList.FindIndex(bounds => bounds.Intersects(item)) != -1;
            }));

            return hallwaysBoundsList;
        }

        private GameObject createRoomObject(Vector3 position, Vector3 size)
        {
            GameObject obj = new GameObject("Seed");
            obj.transform.position = position;

            // 物理シミュレーション用コンポーネントのセットアップ
            Rigidbody2D rigidbody = obj.AddComponent<Rigidbody2D>();
            rigidbody.gravityScale = 0;
            rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;

            BoxCollider2D collider = obj.AddComponent<BoxCollider2D>();
            collider.size = size;

#if UNITY_EDITOR
            obj.AddComponent<MapGenerationSeed>();
#endif
            return obj;
        }
    }

    // 三角形
    public struct Triangle
    {
        public Vector2[] vertices;

        public List<Edge> Edges { get { return getEdges(); } }

        public Triangle(Vector2[] _vertices)
        {
            vertices = _vertices;
        }

        public Triangle(Vector2 a, Vector2 b, Vector2 c)
        {
            vertices = new Vector2[3];
            vertices[0] = a;
            vertices[1] = b;
            vertices[2] = c;
        }

        public bool hasVertex(Vector2 point)
        {
            return vertices.Where(vtx => vtx.Equals(point)).Count() > 0;
        }

        // 指定した辺を含むか
        public bool hasEdge(Edge edge)
        {
            var v1 = vertices.Where(vtx => vtx.Equals(edge.start));
            var v2 = vertices.Where(vtx => vtx.Equals(edge.end));

            return (v1.Count() > 0) && (v2.Count() > 0);
        }

        // 指定した点が三角形に含まれるか
        public bool IsIntersect(Vector2 point)
        {
            Vector3 ca, ap, bc, cp, ab, bp;

            ca = vertices[0] - vertices[2];
            bc = vertices[2] - vertices[1];
            ab = vertices[1] - vertices[0];

            ap = point - vertices[0];
            bp = point - vertices[1];
            cp = point - vertices[2];

            // 
            float sign = Mathf.Sign(Vector3.Cross(ca, ap).z);
            return sign == Mathf.Sign(Vector3.Cross(bc, cp).z) && sign == Mathf.Sign(Vector3.Cross(ab, bp).z);
        }

        public Vector2 GetVertexExcludingEdge(Edge edge)
        {
            var vertex = vertices.Where(vtx => !vtx.Equals(edge.start) && !vtx.Equals(edge.end)).First();
            return vertex;
        }

        private List<Edge> getEdges()
        {
            List<Edge> edges = new List<Edge>();

            edges.Add(new Edge(vertices[0], vertices[1]));
            edges.Add(new Edge(vertices[1], vertices[2]));
            edges.Add(new Edge(vertices[2], vertices[0]));

            return edges;
        }

        public void Draw(Color lineColor, float duration = 0)
        {
            Debug.DrawLine(vertices[0], vertices[1], lineColor, duration);
            Debug.DrawLine(vertices[1], vertices[2], lineColor, duration);
            Debug.DrawLine(vertices[2], vertices[0], lineColor, duration);
        }
    }

    // 辺
    public struct Edge
    {
        public Vector2 start;
        public Vector2 end;

        public List<Vector2> Vertices { get { return getVertices(); } }

        public Edge(Vector2 _start, Vector2 _end)
        {
            start = _start;
            end = _end;
        }

        private List<Vector2> getVertices()
        {
            var vertices = new List<Vector2>();

            vertices.Add(start);
            vertices.Add(end);

            return vertices;
        }
    }

    // 円
    public struct Circle
    {
        public Vector2 center;
        public float radius;

        public Circle(Vector2 _center, float _radius)
        {
            center = _center;
            radius = _radius;
        }

        // 指定した点が円に含まれるか
        public bool IsIntersect(Vector2 point)
        {
            return Vector2.Distance(center, point) < radius;
        }
    }

    // 木
    public class Tree
    {
        HashSet<Vector2> vertices = new HashSet<Vector2>();
        HashSet<Edge> edges = new HashSet<Edge>();

        public List<Edge> Edges { get { return edges.ToList(); } }

        public bool HasEdge(Edge edge)
        {
            return edges.Contains(edge);
        }

        public bool HasVertex(Vector2 vertex)
        {
            return vertices.Contains(vertex);
        }

        public void AddEdge(Edge edge)
        {
            edges.Add(edge);
        }

        public void AddVertex(Vector2 vertex)
        {
            vertices.Add(vertex);
        }

        public void MergeWith(Tree other)
        {
            vertices.UnionWith(other.vertices);
            edges.UnionWith(other.edges);
        }
    }

    public static class Delaunay
    {
        public static List<Triangle> Triangulate(List<Vector3> points)
        {
            List<Triangle> triangles = new List<Triangle>();

            // 点郡を包含する大きな矩形を計算する
            Vector2 max = new Vector2(points.Select(p => p.x).Max(), points.Select(p => p.y).Max());
            Vector2 min = new Vector2(points.Select(p => p.x).Min(), points.Select(p => p.y).Min());
            Bounds bounds = new Bounds();
            bounds.SetMinMax(min, max);

            // Debug
            Debug.DrawLine(bounds.min, new Vector2(bounds.max.x, bounds.min.y), Color.red);
            Debug.DrawLine(new Vector2(bounds.min.x, bounds.max.y), bounds.max, Color.red);

            // 矩形を包含する三角形とその頂点を初期状態とする
            var largeTriangle = GetHugeTriangle(bounds);
            triangles.Add(largeTriangle);

            Stack<Vector3> point_stack = new Stack<Vector3>(points);
            while (point_stack.Count > 0)
            {
                // 頂点を取り出す
                Vector2 point = point_stack.Pop();

                // 頂点を含む三角形を見つける
                var hit_triangles = triangles.FindAll(item => item.IsIntersect(point));

                // 
                Stack<Edge> edge_stack = new Stack<Edge>();
                foreach (var ht in hit_triangles)
                {
                    // 三角形の辺をスタックに積む
                    ht.Edges.ForEach(edge => edge_stack.Push(edge));

                    // 該当の三角形をリストから削除
                    triangles.Remove(ht);

                    // 追加した頂点から構成される3つの三角形を追加
                    var A = ht.vertices[0];
                    var B = ht.vertices[1];
                    var C = ht.vertices[2];

                    var triangle_1 = new Triangle(A, B, point);
                    var triangle_2 = new Triangle(B, C, point);
                    var triangle_3 = new Triangle(C, A, point);

                    triangles.Add(triangle_1);
                    triangles.Add(triangle_2);
                    triangles.Add(triangle_3);
                }

                while (edge_stack.Count > 0)
                {
                    var edge = edge_stack.Pop();

                    // 該当の辺を共有する三角形のリスト
                    var common_edge_triangles = triangles.Where(tri => tri.hasEdge(edge)).ToList();
                    if (common_edge_triangles.Count() < 2)
                    {
                        // 辺を共有する三角形が２つ見つからなければスキップ
                        continue;
                    }

                    var triangle_ABC = common_edge_triangles[0];
                    var triangle_ABD = common_edge_triangles[1];

                    // 選ばれた三角形が同一のものの場合はそれを削除して次へ
                    if (triangle_ABC.Equals(triangle_ABD))
                    {
                        triangles.Remove(triangle_ABC);
                        triangles.Remove(triangle_ABD);
                        continue;
                    }

                    var point_A = edge.start;
                    var point_B = edge.end;
                    var point_C = triangle_ABC.GetVertexExcludingEdge(edge);
                    var point_D = triangle_ABD.GetVertexExcludingEdge(edge);

                    // 頂点Dが三角形ABCの外接円に含まれてるか判定
                    var circle = GetCircumscribedCircleOfTriangle(triangle_ABC);
                    if (circle.IsIntersect(point_D))
                    {
                        // 三角形リストから三角形を削除
                        triangles.Remove(triangle_ABC);
                        triangles.Remove(triangle_ABD);

                        // 共有辺をflipしてできる三角形を新しく三角形郡に追加
                        var triangle_ACD = new Triangle(point_A, point_C, point_D);
                        var triangle_BCD = new Triangle(point_B, point_C, point_D);

                        triangles.Add(triangle_ACD);
                        triangles.Add(triangle_BCD);

                        triangle_ACD.Edges.Where(e => !e.Equals(edge)).ToList().ForEach(e => edge_stack.Push(e));
                        triangle_BCD.Edges.Where(e => !e.Equals(edge)).ToList().ForEach(e => edge_stack.Push(e));
                    }
                }
            }

            // 初期状態の三角形とその頂点を含む三角形を除外
            triangles.Remove(largeTriangle);
            foreach (var vertex in largeTriangle.vertices)
            {
                triangles = triangles.Where(tri => !tri.hasVertex(vertex)).ToList();
            }

            return triangles;
        }

        // 矩形を包含する三角形を取得する
        public static Triangle GetHugeTriangle(Bounds bounds)
        {
            Vector2[] v = new Vector2[3];

            float r = bounds.extents.magnitude;
            Vector2 c = bounds.center;

            // 頂点 (左上、右上、下)
            v[0] = new Vector2(c.x - Mathf.Sqrt(3) * r, c.y + r);
            v[1] = new Vector2(c.x + Mathf.Sqrt(3) * r, c.y + r);
            v[2] = new Vector2(c.x, c.y - 2 * r);

            return new Triangle(v);
        }

        // 三角形の外接円を取得する
        public static Circle GetCircumscribedCircleOfTriangle(Triangle triangle)
        {
            Vector2 v1 = triangle.vertices[0];
            Vector2 v2 = triangle.vertices[1];
            Vector2 v3 = triangle.vertices[2];
            Vector2 v12 = new Vector2(Mathf.Pow(v1.x, 2), Mathf.Pow(v1.y, 2));
            Vector2 v22 = new Vector2(Mathf.Pow(v2.x, 2), Mathf.Pow(v2.y, 2));
            Vector2 v32 = new Vector2(Mathf.Pow(v3.x, 2), Mathf.Pow(v3.y, 2));

            //x = { (y3 - y1)(x22 - x12 + y22 - y12) + (y1 - y2)(x32 - x12 + y32 - y12) } / c
            //y = { (x1 - x3)(x22 - x12 + y22 - y12) + (x2 - x1)(x32 - x12 + y32 - y12) } / c
            float _x = (v22.x - v12.x + v22.y - v12.y);
            float _y = (v32.x - v12.x + v32.y - v12.y);

            //c = 2 { (x2 - x1)(y3 - y1) - (y2 - y1)(x3 - x1) }
            float c = 2 * ((v2.x - v1.x) * (v3.y - v1.y) - (v2.y - v1.y) * (v3.x - v1.x));

            Vector2 center;
            center.x = ((v3.y - v1.y) * _x + (v1.y - v2.y) * _y) / c;
            center.y = ((v1.x - v3.x) * _x + (v2.x - v1.x) * _y) / c;

            float radius = Vector2.Distance(center, v1);

            return new Circle(center, radius);
        }
    }

    public static class Kruskal
    {
        public static List<Edge> CalculateSpanningTree(List<Edge> edges)
        {
            // HashSet に格納することで、重複の削除とソートを行う
            SortedSet<Edge> edge_set = new SortedSet<Edge>(edges, new ByDistance());

            // 森(木の集合) F を生成する
            // 頂点1個だけからなる木が頂点の個数だけ存在する
            List<Tree> F = new List<Tree>();
            List<Vector2> edge_vertices = edge_set.Aggregate(new List<Vector2>(), (sum, next) => sum.Concat(next.Vertices).ToList());
            HashSet<Vector2> vertices = new HashSet<Vector2>(edge_vertices);
            foreach (var vertex in vertices)
            {
                Tree tree = new Tree();
                tree.AddVertex(vertex);
                F.Add(tree);
            }

            // グラフの全ての辺を含む集合 E を生成する
            Stack<Edge> E = new Stack<Edge>(edge_set);

            while (E.Count > 0)
            {
                // 重みが最小の辺を取り出す
                var e = E.Pop();
                //Debug.Log(Vector2.Distance(e.start, e.end));

                // 辺 e につながっている頂点 u,v が所属する木を見つける
                var tree_u = F.Where(tree => tree.HasVertex(e.start)).First();
                var tree_v = F.Where(tree => tree.HasVertex(e.end)).First();

                // 別々の木に所属しているなら、二つの木を連結する
                if (!tree_u.Equals(tree_v))
                {
                    tree_u.MergeWith(tree_v);
                    tree_u.AddEdge(e);
                    F.Remove(tree_v);
                }
            }

            // 最終的に森 F はただひとつの木を含む
            Debug.Assert(F.Count() == 1);

            return F.First().Edges;
        }

        public class ByDistance : IComparer<Edge>
        {
            public int Compare(Edge a, Edge b)
            {
                if (a.Equals(b))
                {
                    return 0;
                }
                else
                {
                    float dist_a = Vector2.Distance(a.start, a.end);
                    float dist_b = Vector2.Distance(b.start, b.end);

                    return (int)Mathf.Sign(dist_b - dist_a);
                }
            }
        }
    }
}
