using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
