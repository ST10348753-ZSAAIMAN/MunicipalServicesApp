using System;
using System.Collections.Generic;

namespace MunicipalServicesApp.Structures.Graphs
{
    /// <summary>
    /// Prim's algorithm for MST on a weighted undirected Graph.
    /// Framework-safe tuple access via Item1/Item2/Item3.
    /// </summary>
    public static class MinimumSpanningTree
    {
        // Returns: edges = list of (u,v,w), total = sum of weights
        public static (List<(int, int, double)> edges, double total) Prim(Graph g, int start = 0)
        {
            int n = g.VertexCount;
            var used = new bool[n];
            var resultEdges = new List<(int, int, double)>();
            double total = 0;

            // Min-heap (simulated with SortedSet) of (w,u,v)
            var pq = new SortedSet<(double, int, int)>(
                Comparer<(double, int, int)>.Create((a, b) =>
                {
                    // Compare by weight, then u, then v
                    if (a.Item1 != b.Item1) return a.Item1.CompareTo(b.Item1);
                    if (a.Item2 != b.Item2) return a.Item2.CompareTo(b.Item2);
                    return a.Item3.CompareTo(b.Item3);
                })
            );

            void AddFrontier(int u)
            {
                used[u] = true;
                foreach (var e in g.Neighbours(u))
                {
                    if (!used[e.To])
                    {
                        // (w, u, v)
                        pq.Add((e.W, u, e.To));
                    }
                }
            }

            AddFrontier(start);

            while (pq.Count > 0 && resultEdges.Count < n - 1)
            {
                var best = pq.Min;          // (w,u,v)
                pq.Remove(best);

                int v = best.Item3;
                if (used[v]) continue;

                int u = best.Item2;
                double w = best.Item1;

                resultEdges.Add((u, v, w));
                total += w;
                AddFrontier(v);
            }

            return (resultEdges, total);
        }
    }
}
