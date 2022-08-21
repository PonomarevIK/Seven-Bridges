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

        public double Diameter => 40;
        private Point grabPoint;
        public List<BaseEdge> Edges = new List<BaseEdge>();

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

        public Vertex(double x, double y)
        {
            DataContext = this;
            position = new Point(x - Diameter / 2, y - Diameter / 2);
            Content = Index++;
        }

        #region Edges
        public bool IsConnected(Vertex vertex)
        {
            if (vertex == this) return true;
            return Edges.Any((edge) => (edge.V1 == vertex && !(edge is DirectedEdge)) || (edge.V2 == vertex));
        }

        public bool TryAddEdgeTo(BaseEdge edge)
        {
            if (edge.V1.IsConnected(this)) return false;
            Edges.Add(edge);
            edge.V2 = this;

            return true;
        }
        public bool TryAddEdgeFrom(BaseEdge edge)
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
            Background = Brushes.LightCyan;
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
            Background = Brushes.LightBlue;
            MouseMove -= DragMouseMove;
            MouseLeftButtonUp -= DragStop;
            Canvas.SetZIndex(this, 0);
        }
        #endregion

        public void Delete()
        {
            foreach (var edge in Edges)
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
