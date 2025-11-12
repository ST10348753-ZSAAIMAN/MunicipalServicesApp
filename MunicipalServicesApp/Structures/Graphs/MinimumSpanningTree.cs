using System;
using System.Collections.Generic;
using System.Linq;

namespace MunicipalServicesApp.Structures.Graphs
{
    /// <summary>
    /// Prim's algorithm for MST on a weighted undirected Graph.
    /// </summary>
    public static class MinimumSpanningTree
    {
        public static (List<(int u, int v, double w)> edges, double total) Prim(Graph g, int start = 0)
        {
            int n = g.VertexCount;
            var used = new bool[n];
            var res = new List<(int, int, double)>();
            double total = 0;

            // min-heap of (w,u,v)
            var pq = new SortedSet<(double w, int u, int v)>(Comparer<(double, int, int)>.Create(
                (a, b) => a.w != b.w ? a.w.CompareTo(b.w) : (a.u != b.u ? a.u.CompareTo(b.u) : a.v.CompareTo(b.v))));

            void add(int u)
            {
                used[u] = true;
                foreach (var e in g.Neighbours(u))
                    if (!used[e.To]) pq.Add((e.W, u, e.To));
            }

            add(start);
            while (pq.Count > 0 && res.Count < n - 1)
            {
                var best = pq.Min; pq.Remove(best);
                if (used[best.v]) continue;
                res.Add((best.u, best.v, best.w)); total += best.w;
                add(best.v);
            }

            return (res, total);
        }
    }
}
