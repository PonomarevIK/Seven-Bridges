using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace Seven_Bridges.Controls
{
    public class Vertex : UserControl, INotifyPropertyChanged
    {
        /// <summary>
        /// Keeps track of how many vertices were created, and gives each new one a unique index.
        /// </summary>
        private static int Index = 1;

        /// <summary>
        /// Diameter of an ellipse representing this vertex on canvas.
        /// </summary>
        public double Diameter => 40;
        
        /// <summary>
        /// Relative position of where control has been grabbed before being dragged.
        /// </summary>
        private Point grabPoint;

        /// <summary>
        /// List of all edges to and from this vertex.
        /// </summary>
        public List<Edge> Edges = new List<Edge>();

        // Control position on canvas
        private Point position;
        public double X
        {
            get => position.X;
            set
            {
                position.X = value;
                OnPropertyChanged("X");
                OnPropertyChanged("CenterX");
            }
        }
        public double Y
        {
            get => position.Y;
            set
            {
                position.Y = value;
                OnPropertyChanged("Y");
                OnPropertyChanged("CenterY");
            }
        }
        public double CenterX => position.X + Diameter / 2;
        public double CenterY => position.Y + Diameter / 2;
        
        static Vertex()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Vertex), new FrameworkPropertyMetadata(typeof(Vertex)));
        }
        /// <summary>
        /// Create a new vertex with specified position (x, y).
        /// </summary>
        public Vertex(double x, double y)
        {
            DataContext = this;
            position = new Point(x - Diameter / 2, y - Diameter / 2);
            Content = Index++;
        }

        /// <summary>
        /// Iterates through every vertex connected to this one.
        /// </summary>
        /// <param name="ignoreDirection">If true, inward connections will be considered neighbors as well.</param>
        /// <returns></returns>
        public IEnumerable<Vertex> GetNeighbors(bool ignoreDirection = false)
        {
            foreach (Edge edge in Edges)
            {
                if (edge.V2 != this) yield return edge.V2;
                else if ((!edge.IsDirected || ignoreDirection) && edge.V1 != this) yield return edge.V1;
            }
        }
        /// <summary>
        /// Iterates through every vertex connected to this one and yields (neighborVertex, distance) tuples.
        /// </summary>
        /// <param name="ignoreDirection">If true, inward connections will be considered neighbors as well.</param>
        /// <returns></returns>
        public IEnumerable<(Vertex, double)> GetNeighborsWithWeights(bool ignoreDirection = false)
        {
            foreach (Edge edge in Edges)
            {
                if (edge.V2 != this) yield return (edge.V2, edge.Weight);
                else if ((!edge.IsDirected || ignoreDirection) && edge.V1 != this) yield return (edge.V1, edge.Weight);
            }
        }
        /// <summary>
        /// Iterates through every vertex connected to this one and yields (neighborVertex, connectingEdge) tuples.
        /// </summary>
        /// <param name="ignoreDirection">If true, inward connections will be considered neighbors as well.</param>
        /// <returns></returns>
        public IEnumerable<(Vertex, Edge)> GetNeighborsWithEdges(bool ignoreDirection = false)
        {
            foreach (Edge edge in Edges)
            {
                if (edge.V2 != this) yield return (edge.V2, edge);
                else if ((!edge.IsDirected || ignoreDirection) && edge.V1 != this) yield return (edge.V1, edge);
            }
        }

        #region Edges
        public bool IsConnectedTo(Vertex vertex)
        {
            if (vertex == this) return true;
            return Edges.Any((edge) => (edge.V1 == vertex && !(edge.IsDirected)) || (edge.V2 == vertex));
        }
        public bool IsConnectedTo(Vertex vertex, out Edge connectingEdge)
        {
            connectingEdge = null;
            if (vertex == this) return true;
            foreach (Edge edge in Edges)
            {
                if ((edge.V1 == vertex && !(edge.IsDirected)) || (edge.V2 == vertex))
                {
                    connectingEdge = edge;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Tries to connect an edge TO this vertex. Retirns true on success.
        /// </summary>
        public bool TryAddEdgeHead(Edge edge)
        {
            if (edge.V1.IsConnectedTo(this)) return false;
            else if (this.IsConnectedTo(edge.V1, out Edge connectingEdge))
            {
                connectingEdge.IsDirected = false;
                return false;
            }
            Edges.Add(edge);
            edge.V2 = this;

            return true;
        }
        /// <summary>
        /// Tries to connect an edge FROM this vertex. Retirns true on success.
        /// </summary>
        public bool TryAddEdgeTail(Edge edge)
        {
            Edges.Add(edge);
            edge.V1 = this;

            return true;
        }
        #endregion

        #region Drag
        public void DragStart(Point grabPoint)
        {
            CaptureMouse();
            this.grabPoint = grabPoint;
            MouseMove += DragOnMouseMove;
            MouseLeftButtonUp += DragStop;
            Canvas.SetZIndex(this, 1);
        }
        private void DragOnMouseMove(object sender, MouseEventArgs eventArgs)
        {
            var parent = Parent as GraphCanvas;
            var mousePosition = eventArgs.GetPosition(parent);

            if (mousePosition.X - grabPoint.X < 0)
                X = 0;
            else if (mousePosition.X + Diameter - grabPoint.X > parent.Width)
                X = parent.Width - Diameter;
            else
                X = mousePosition.X - grabPoint.X;

            if (mousePosition.Y - grabPoint.Y < 0)
                Y = 0;
            else if (mousePosition.Y + Diameter - grabPoint.Y > parent.Height)
                Y = parent.Height - Diameter;
            else
                Y = mousePosition.Y - grabPoint.Y;
        }
        private void DragStop(object sender, MouseEventArgs eventArgs)
        {
            ReleaseMouseCapture();
            MouseMove -= DragOnMouseMove;
            MouseLeftButtonUp -= DragStop;
            Canvas.SetZIndex(this, 0);
        }
        #endregion

        public void Delete()
        {
            foreach (Edge edge in Edges)
            {
                edge.Delete(this);
            }
            Edges.Clear();
            (Parent as Panel).Children.Remove(this);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
