using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Proto2D
{
    public static class KruskalSpanningTree
    {
        public class Tree
        {
            HashSet<Vector2> vertices = new HashSet<Vector2>();
            HashSet<DelaunayTriangulation.Edge> edges = new HashSet<DelaunayTriangulation.Edge>();

            public List<DelaunayTriangulation.Edge> Edges { get { return edges.ToList(); } } 

            public bool HasEdge(DelaunayTriangulation.Edge edge)
            {
                return edges.Contains(edge);
            }

            public bool HasVertex(Vector2 vertex)
            {
                return vertices.Contains(vertex);
            }

            public void AddEdge(DelaunayTriangulation.Edge edge)
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

        public static List<DelaunayTriangulation.Edge> Calculate(List<DelaunayTriangulation.Edge> edges)
        {
            // HashSet に格納することで、重複の削除とソートを行う
            SortedSet<DelaunayTriangulation.Edge> edge_set = new SortedSet<DelaunayTriangulation.Edge>(edges, new ByDistance());

            // 森(木の集合) F を生成する
            // 頂点1個だけからなる木が頂点の個数だけ存在する
            List<Tree> F = new List<Tree>();
            List<Vector2> edge_vertices = edge_set.Aggregate(new List<Vector2>(), (sum, next) => sum.Concat(next.Vertices).ToList());
            HashSet<Vector2> vertices = new HashSet<Vector2>(edge_vertices);
            foreach(var vertex in vertices)
            {
                Tree tree = new Tree();
                tree.AddVertex(vertex);
                F.Add(tree);
            }

            // グラフの全ての辺を含む集合 E を生成する
            Stack<DelaunayTriangulation.Edge> E = new Stack<DelaunayTriangulation.Edge>(edge_set);

            while (E.Count > 0)
            {
                // 重みが最小の辺を取り出す
                var e = E.Pop();
                Debug.Log(Vector2.Distance(e.start, e.end));

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

        public class ByDistance : IComparer<DelaunayTriangulation.Edge>
        {
            public int Compare(DelaunayTriangulation.Edge a, DelaunayTriangulation.Edge b)
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
