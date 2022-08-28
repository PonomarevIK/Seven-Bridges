using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace Seven_Bridges.Controls
{
    public class Vertex : UserControl, INotifyPropertyChanged
    {
        private static int Index = 1;
        public bool IsVisited = false;

        public double Diameter => 40;
        private Point grabPoint;
        public List<Edge> Edges = new List<Edge>();

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
        public Vertex(double x, double y)
        {
            DataContext = this;
            position = new Point(x - Diameter / 2, y - Diameter / 2);
            Content = Index++;
        }

        public IEnumerable<Vertex> GetNeighbors()
        {
            foreach (Edge edge in Edges)
            {
                if (edge.V2 != this) yield return edge.V2;
                else if (!edge.IsDirected && edge.V1 != this) yield return edge.V1;
            }
        }

        #region Edges
        public bool IsConnected(Vertex vertex)
        {
            if (vertex == this) return true;
            return Edges.Any((edge) => (edge.V1 == vertex && !(edge.IsDirected)) || (edge.V2 == vertex));
        }
        public bool IsConnected(Vertex vertex, out Edge connectingEdge)
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

        public bool TryAddEdgeTo(Edge edge)
        {
            if (edge.V1.IsConnected(this)) return false;
            else if (this.IsConnected(edge.V1, out Edge connectingEdge))
            {
                connectingEdge.IsDirected = false;
                return false;
            }
            Edges.Add(edge);
            edge.V2 = this;

            return true;
        }
        public bool TryAddEdgeFrom(Edge edge)
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
            MouseMove += DragMouseMove;
            MouseLeftButtonUp += DragStop;
            Canvas.SetZIndex(this, 1);
        }
        private void DragMouseMove(object sender, MouseEventArgs eventArgs)
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
            MouseMove -= DragMouseMove;
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
