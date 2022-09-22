using Seven_Bridges.Controls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Seven_Bridges
{
    public static class Algorithms
    {
        // Slightly shorter version of Array.IndexOf()
        private static int IndexOf(Vertex vertex, Vertex[] vertices) => Array.IndexOf(vertices, vertex);
        public static double TotalDistance(LinkedList<Edge> path) => path.Sum((Edge e) => e.Weight);

        /// <summary> Breadth Fist Search algorithm </summary>
        /// <param name="start">Vertex from which search starts</param>
        /// <param name="vertices">Array of all vertices on a graph</param>
        /// <param name="isVisited">Boolean Array where isVisited[i] = true if vertices[i] has been encountered during search</param>
        /// <param name="ignoreDirection">If true, direction of edges will be ignored and any vertex pair will be considered connected if there is an edge between them</param>
        /// <param name="end">Optional parameter to check wether a specified vertex has been reached or not</param>
        /// <returns>If given 'end' parameter returns true if that vertex has been reached. False otherwise</returns>
        private static bool BFS(Vertex start, Vertex[] vertices, bool[] isVisited, bool ignoreDirection, Vertex end = null)
        {
            bool TryVisit(Vertex vertex)
            {
                int index = IndexOf(vertex, vertices);
                bool visited = isVisited[index];
                isVisited[index] = true;
                return visited;
            }

            var serarchQueue = new Queue<Vertex>();
            serarchQueue.Enqueue(start);
            TryVisit(start);

            while(serarchQueue.Count > 0)
            {
                foreach (Vertex neighbor in serarchQueue.Dequeue().GetNeighbors(ignoreDirection))
                {
                    if (TryVisit(neighbor)) continue;
                    serarchQueue.Enqueue(neighbor);
                }
            }
            return end == null ? false : isVisited[IndexOf(end, vertices)];
        }

        /// <returns>A number of connected components in a graph</returns>
        public static int ComponentCount(GraphCanvas canvas)
        {
            int componentCount = 0;
            Vertex[] vertices = canvas.GetArrayOfVertex();
            bool[] isVisited = new bool[vertices.Length];

            for (int i = 0; i < vertices.Length; i++)
            {
                if (!isVisited[i])
                {
                    BFS(vertices[i], vertices, isVisited, true);
                    componentCount++;
                }
            }

            return componentCount;
        }

        /// <returns>If a path exists from 'start' to 'end' returns the shortest one as a linked list of edges. Otherwise returns null.</returns>
        public static LinkedList<Edge> Dijkstra(Vertex start, Vertex end, GraphCanvas canvas)
        {
            var vertices = canvas.GetArrayOfVertex();
            var traversalQueue = new Queue<Vertex>();
            LinkedList<Edge>[] paths = new LinkedList<Edge>[vertices.Length];

            paths[IndexOf(start, vertices)] = new LinkedList<Edge>();
            traversalQueue.Enqueue(start);
            while (traversalQueue.Count > 0)
            {
                var currentVertex = traversalQueue.Dequeue();
                int currentVertexIndex = IndexOf(currentVertex, vertices);
                foreach ((Vertex neighbor, Edge edge) in currentVertex.GetNeighborsWithEdges(false))
                {
                    int neighborIndex = IndexOf(neighbor, vertices);
                    if (paths[neighborIndex] != null && TotalDistance(paths[neighborIndex]) <= TotalDistance(paths[currentVertexIndex]) + edge.Weight) continue;
                    if (paths[neighborIndex] == null || TotalDistance(paths[neighborIndex]) > TotalDistance(paths[currentVertexIndex]) + edge.Weight)
                    {
                        paths[neighborIndex] = new LinkedList<Edge>(paths[currentVertexIndex]);
                        paths[neighborIndex].AddLast(edge);
                    }
                    traversalQueue.Enqueue(neighbor);
                }
            }

            return paths[IndexOf(end, vertices)];
        }
    }
}
