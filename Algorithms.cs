using Seven_Bridges.Controls;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Seven_Bridges
{
    public static class Algorithms
    {

        private static int IndexOf(Vertex vertex, Vertex[] vertices) => Array.IndexOf(vertices, vertex);
        

        private static bool BFS(Vertex start, Vertex[] vertices, bool[] isVisited, Vertex end = null, bool ignoreDirection = false)
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

        public static int ComponentCount(GraphCanvas canvas)
        {
            int componentCount = 0;
            Vertex[] vertices = canvas.GetArrayOfVertex();
            bool[] isVisited = new bool[vertices.Length];

            for (int i = 0; i < vertices.Length; i++)
            {
                if (!isVisited[i])
                {
                    BFS(vertices[i], vertices, isVisited, null, true);
                    componentCount++;
                }
            }

            return componentCount;
        }

        public static double Dijkstra(Vertex start, Vertex end, GraphCanvas canvas)
        {
            var vertices = canvas.GetArrayOfVertex();
            var traversalQueue = new Queue<Vertex>();
            double[] distances = new double[vertices.Length];
            for (int i = 0; i < distances.Length; i++) distances[i] = -1;

            traversalQueue.Enqueue(start);
            distances[IndexOf(start, vertices)] = 0;
            while (traversalQueue.Count > 0)
            {
                var currentVertex = traversalQueue.Dequeue();
                int currentVertexIndex = IndexOf(currentVertex, vertices);
                foreach ((Vertex neighbor, double weight) in currentVertex.GetNeighborsWithWeights(false))
                {
                    int neighborIndex = IndexOf(neighbor, vertices);
                    if (distances[neighborIndex] != -1 && distances[neighborIndex] <= distances[currentVertexIndex] + weight) continue;
                    if (distances[neighborIndex] == -1 || distances[neighborIndex] > distances[currentVertexIndex] + weight)
                    {
                        distances[neighborIndex] = distances[currentVertexIndex] + weight;
                    }
                    traversalQueue.Enqueue(neighbor);
                }
            }
            return distances[IndexOf(end, vertices)];
        }
    }
}
