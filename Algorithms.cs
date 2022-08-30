using Seven_Bridges.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Seven_Bridges
{
    public static class Algorithms
    {
        private static void BFS(Vertex start, Vertex[] vertices, bool[] isVisited)
        {
            int IndexOf(Vertex vertex) => Array.IndexOf(vertices, vertex);
            bool TryVisit(Vertex vertex)
            {
                int index = IndexOf(vertex);
                bool visited = isVisited[index];
                isVisited[index] = true;
                return visited;
            }

            var serarchQueue = new Queue<Vertex>();
            serarchQueue.Enqueue(start);
            TryVisit(start);

            while(serarchQueue.Count > 0)
            {
                foreach (Vertex neighbor in serarchQueue.Dequeue().GetNeighbors(true))
                {
                    if (TryVisit(neighbor)) continue;
                    serarchQueue.Enqueue(neighbor);
                }
            }
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
                    BFS(vertices[i], vertices, isVisited);
                    componentCount++;
                }
            }

            return componentCount;
        }
    }
}
