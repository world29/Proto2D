using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Proto2D
{
    public static class DelaunayTriangulation
    {
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

                var circle = GetCircumscribedCircleOfTriangle(this);
                DebugUtility.DrawEllipse(circle.center, Vector3.forward, Vector3.up, circle.radius, circle.radius, 32, new Color(1,0,1,.2f), 1);
            }
        }

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

        public static List<Triangle> Calculate(List<Vector2> points)
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

            Stack<Vector2> point_stack = new Stack<Vector2>(points);
            while (point_stack.Count > 0)
            {
                // 頂点を取り出す
                Vector2 point = point_stack.Pop();

                // 頂点を含む三角形を見つける
                var hit_triangles = triangles.FindAll(item => item.IsIntersect(point));

                // 
                Stack<Edge> edge_stack = new Stack<Edge>();
                foreach(var ht in hit_triangles)
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

        public static class DebugUtility
        {
            // https://forum.unity.com/threads/solved-debug-drawline-circle-ellipse-and-rotate-locally-with-offset.331397/#post-3184954
            public static void DrawEllipse(Vector3 pos, Vector3 forward, Vector3 up, float radiusX, float radiusY, int segments, Color color, float duration = 0)
            {
                float angle = 0f;
                Quaternion rot = Quaternion.LookRotation(forward, up);
                Vector3 lastPoint = Vector3.zero;
                Vector3 thisPoint = Vector3.zero;

                for (int i = 0; i < segments + 1; i++)
                {
                    thisPoint.x = Mathf.Sin(Mathf.Deg2Rad * angle) * radiusX;
                    thisPoint.y = Mathf.Cos(Mathf.Deg2Rad * angle) * radiusY;

                    if (i > 0)
                    {
                        Debug.DrawLine(rot * lastPoint + pos, rot * thisPoint + pos, color, duration);
                    }

                    lastPoint = thisPoint;
                    angle += 360f / segments;
                }
            }
        }


    }
}
