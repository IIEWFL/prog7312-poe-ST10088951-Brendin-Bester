using System;
using System.Collections.Generic;
using System.Linq;

namespace MunicipalServicesApp.DataStructures
{
    // Undirected weighted graph using adjacency lists
    //harikrishnakera (2020). Graph Data Structure in C# | BFS & DFS Code. [online] Learn Coding from Experts. Available at: https://masterdotnet.com/csharp/ds/graphincsharp/.‌
    public class Graph<T>
    {
        private readonly Dictionary<T, List<(T node, double weight)>> adj = new Dictionary<T, List<(T, double)>>();

        public void AddVertex(T v)
        {
            if (!adj.ContainsKey(v)) adj[v] = new List<(T, double)>();
        }

        public void RemoveVertex(T v)
        {
            if (!adj.ContainsKey(v)) return;
            adj.Remove(v);
            foreach (var kv in adj) kv.Value.RemoveAll(e => EqualityComparer<T>.Default.Equals(e.node, v));
        }

        public void AddEdge(T a, T b, double weight = 1.0)
        {
            AddVertex(a); AddVertex(b);
            adj[a].Add((b, weight));
            adj[b].Add((a, weight));
        }

        public IEnumerable<T> Neighbors(T v)
        {
            if (!adj.ContainsKey(v)) return Enumerable.Empty<T>();
            return adj[v].Select(e => e.node);
        }

        public IEnumerable<T> GetVertices() => adj.Keys;

        public IEnumerable<T> BreadthFirstTraversal(T start)
        {
            var result = new List<T>();
            if (!adj.ContainsKey(start)) return result;
            var q = new Queue<T>(); var seen = new HashSet<T>();
            q.Enqueue(start); seen.Add(start);
            while (q.Count > 0)
            {
                var cur = q.Dequeue();
                result.Add(cur);
                foreach (var n in adj[cur].Select(x => x.node))
                    if (seen.Add(n)) q.Enqueue(n);
            }
            return result;
        }

        public IEnumerable<T> DepthFirstTraversal(T start)
        {
            var result = new List<T>();
            if (!adj.ContainsKey(start)) return result;
            var st = new Stack<T>(); var seen = new HashSet<T>();
            st.Push(start);
            while (st.Count > 0)
            {
                var cur = st.Pop();
                if (!seen.Add(cur)) continue;
                result.Add(cur);
                foreach (var n in adj[cur].Select(x => x.node))
                    if (!seen.Contains(n)) st.Push(n);
            }
            return result;
        }

        // Simple Prim's algorithm returning MST edges (a,b,weight)
        public IEnumerable<(T a, T b, double weight)> PrimMstEdges()
        {
            var vertices = adj.Keys.ToList();
            var result = new List<(T, T, double)>();
            if (!vertices.Any()) return result;

            var start = vertices[0];
            var inMst = new HashSet<T> { start };
            var edges = new List<(T a, T b, double w)>();

            void AddEdgesFrom(T v)
            {
                foreach (var e in adj[v])
                    if (!inMst.Contains(e.node)) edges.Add((v, e.node, e.weight));
            }

            AddEdgesFrom(start);

            while (inMst.Count < vertices.Count)
            {
                if (!edges.Any()) break;
                var next = edges.OrderBy(e => e.w).First();
                edges.Remove(next);
                if (inMst.Contains(next.b)) continue;
                result.Add(next);
                inMst.Add(next.b);
                AddEdgesFrom(next.b);
            }

            return result;
        }
    }
}