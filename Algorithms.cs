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
        private static void BFS(GraphCanvas canvas, Vertex start)
        {
            Queue<Vertex> vertices = new Queue<Vertex>();
            vertices.Enqueue(start);
            start.IsVisited = true;

            while(vertices.Count > 0)
            {
                foreach(Vertex neighbor in vertices.Dequeue().GetNeighbors())
                {
                    if (neighbor.IsVisited) continue;
                    vertices.Enqueue(neighbor);
                    neighbor.IsVisited = true;
                }
            }
        }

        public static int ComponentCount(GraphCanvas canvas)
        {
            int count = 0;

            foreach (Vertex vertex in canvas.GetVertices())
            {
                if (!vertex.IsVisited) 
                { 
                    count++;
                    BFS(canvas, vertex);
                }
            }
            Clear(canvas);
            return count;
        }

        private static void Clear(GraphCanvas canvas)
        {
            foreach (Vertex vertex in canvas.GetVertices())
            {
                vertex.IsVisited = false;
            }
        }
    }
}
