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

            public void Draw(Color lineColor, float duration = 0)
            {
                Debug.DrawLine(vertices[0], vertices[1], lineColor, duration);
                Debug.DrawLine(vertices[1], vertices[2], lineColor, duration);
                Debug.DrawLine(vertices[2], vertices[0], lineColor, duration);

                var circle = GetCircumscribedCircleOfTriangle(this);
                DebugUtility.DrawEllipse(circle.center, Vector3.forward, Vector3.up, circle.radius, circle.radius, 32, new Color(1,0,1,.2f), 1);
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

        public static List<Triangle> Triangulation(List<Vector2> points)
        {
            List<Triangle> triangles = new List<Triangle>();

            // 点郡を包含する大きな矩形を計算する
            Vector2 max = new Vector2(points.Select(p => p.x).Max(), points.Select(p => p.y).Max());
            Vector2 min = new Vector2(points.Select(p => p.x).Min(), points.Select(p => p.y).Min());
            Bounds bounds = new Bounds();
            bounds.SetMinMax(min, max);

            // 矩形を包含する三角形を初期値とする
            var initTriangle = GetHugeTriangle(bounds);
            triangles.Add(initTriangle);

            // 三角形分割に頂点を追加する
            foreach(Vector2 point in points)
            {
                // 追加した頂点を含む三角形を見つける
                int index = triangles.FindIndex(item => item.IsIntersect(point));
                Debug.Assert(index != -1);

                Triangle tri = triangles[index];

                // ABC に X を追加して、XAB, XBC, XCA とする
                Triangle xab = new Triangle(point, tri.vertices[0], tri.vertices[1]);
                Triangle xbc = new Triangle(point, tri.vertices[1], tri.vertices[2]);
                Triangle xca = new Triangle(point, tri.vertices[2], tri.vertices[0]);

                triangles.Remove(tri);
                triangles.Add(xab);
                triangles.Add(xbc);
                triangles.Add(xca);
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
