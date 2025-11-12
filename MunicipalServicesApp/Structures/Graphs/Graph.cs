using System;
using System.Collections.Generic;

namespace MunicipalServicesApp.Structures.Graphs
{
    /// <summary>
    /// Weighted undirected graph (adjacency list). Supports BFS and DFS.
    /// </summary>
    public class Graph
    {
        public class Edge { public int To; public double W; public Edge(int to, double w) { To = to; W = w; } }
        private readonly List<List<Edge>> _adj = new List<List<Edge>>();
        private readonly List<string> _names = new List<string>();

        public int AddVertex(string name) { _names.Add(name); _adj.Add(new List<Edge>()); return _names.Count - 1; }
        public void AddUndirectedEdge(int u, int v, double w) { _adj[u].Add(new Edge(v, w)); _adj[v].Add(new Edge(u, w)); }

        public IEnumerable<string> BFS(int start)
        {
            var seen = new bool[_adj.Count];
            var q = new Queue<int>();
            seen[start] = true; q.Enqueue(start);
            while (q.Count > 0)
            {
                int u = q.Dequeue();
                yield return _names[u];
                foreach (var e in _adj[u]) if (!seen[e.To]) { seen[e.To] = true; q.Enqueue(e.To); }
            }
        }

        public IEnumerable<string> DFS(int start)
        {
            var seen = new bool[_adj.Count];
            foreach (var name in DfsRec(start, seen)) yield return name;
        }
        private IEnumerable<string> DfsRec(int u, bool[] seen)
        {
            seen[u] = true; yield return _names[u];
            foreach (var e in _adj[u]) if (!seen[e.To]) foreach (var k in DfsRec(e.To, seen)) yield return k;
        }

        public int VertexCount => _adj.Count;
        public string NameOf(int i) => _names[i];
        public IEnumerable<(int u, int v, double w)> AllEdges()
        {
            var s = new HashSet<(int, int)>();
            for (int u = 0; u < _adj.Count; u++)
                foreach (var e in _adj[u])
                    if (!s.Contains((e.To, u))) { s.Add((u, e.To)); yield return (u, e.To, e.W); }
        }
        public IReadOnlyList<Edge> Neighbours(int u) => _adj[u];
    }
}
