using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace Seven_Bridges.Controls
{
    public class GraphCanvas : Canvas
    {
        Point prevMousePosition;
        Edge selectedEdge;
        Vertex selectedVertex;
        bool isToolInputBlocked = false;

        int zoomScale = 0;
        const double zoomInFactor = 1.2;
        const double zoomOutFactor = 1 / zoomInFactor;
        const int maxZoomScale = 10;
        const int minZoomScale = -10;

        readonly TransformGroup transformGroup;
        readonly ScaleTransform scaleTransform;
        readonly TranslateTransform translateTransform;

        static GraphCanvas()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GraphCanvas), new FrameworkPropertyMetadata(typeof(GraphCanvas)));
        }
        public GraphCanvas()
        {
            // Visual transformations
            transformGroup = new TransformGroup();
            scaleTransform = new ScaleTransform();
            translateTransform = new TranslateTransform();
            transformGroup.Children.Add(scaleTransform);
            transformGroup.Children.Add(translateTransform);
            RenderTransform = transformGroup;
            // Events
            MouseWheel += Zoom;
            KeyDown += KeyCommand;
        }
        public void Initialize()
        {
            Keyboard.Focus(this);
            scaleTransform.CenterX = Width / 2;
            scaleTransform.CenterY = Height / 2;
        }

        /// <summary>
        /// Get an array of every vertex currently on canvas.
        /// </summary>
        public Vertex[] GetArrayOfVertex()
        {
            int size = VertexCount;
            var vertices = new Vertex[size];
            int i = 0;

            foreach (UIElement element in Children)
            {
                if (element is Vertex vertex) vertices[i++] = vertex;
            }

            return vertices;
        }
        /// <summary>
        /// Number of vertices on canvas.
        /// </summary>
        public int VertexCount
        {
            get
            {
                int count = 0;
                foreach (UIElement element in Children)
                {
                    if (element is Vertex) count++;
                }
                return count;
            }
        }

        #region Tool Selection Events
        public void DragToolSelected()
        {
            MouseLeftButtonDown += CanvasGrab;
        }
        public void DragToolUnselected()
        {
            MouseLeftButtonDown -= CanvasGrab;
            DragStop(null, null);
        }
        public void AddToolSelected()
        {
            MouseLeftButtonDown += AddVertex;
        }
        public void AddToolUnselected()
        {
            MouseLeftButtonDown -= AddVertex;
        }
        public void DeleteToolSelected()
        {
            MouseLeftButtonDown += DeleteItem;
        }
        public void DeleteToolUnselected()
        {
            MouseLeftButtonDown -= DeleteItem;
        }
        public void UndirectedEdgeToolSelected()
        {
            MouseLeftButtonDown += UndirectedEdgeStart;
        }
        public void UndirectedEdgeToolUnselected()
        {
            MouseLeftButtonDown -= UndirectedEdgeStart;
            MouseMove -= ConnectOnMouseMove;
            MouseLeftButtonDown -= ConnectEnd;
            selectedEdge?.Delete();
            selectedEdge = null;
        }
        public void DirectedEdgeToolSelected()
        {
            MouseLeftButtonDown += DirectedEdgeStart;
        }
        public void DirectedEdgeToolUnselected()
        {
            MouseLeftButtonDown -= DirectedEdgeStart;
            MouseMove -= ConnectOnMouseMove;
            MouseLeftButtonDown -= ConnectEnd;
            selectedEdge?.Delete();
            selectedEdge = null;
        }
        public void ShortestPathSelected()
        {
            isToolInputBlocked = true;
            MouseLeftButtonDown += ShortestPathPointSelector;
        }
        #endregion

        private void KeyCommand(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                // + to zoom in
                case Key.Add:
                case Key.OemPlus:
                    Scale(zoomInFactor);
                    break;
                // - to zoom out
                case Key.Subtract:
                case Key.OemMinus:
                    Scale(zoomOutFactor);
                    break;
                // Enter to reset zoom scale and canvas position 
                case Key.Enter:
                    ResetPosition();
                    ResetScale();
                    break;
            }
        }

        public void BlockToolInput() => isToolInputBlocked = true;
        public void UnblockToolInput() => isToolInputBlocked = false;

        #region Drag
        private void CanvasGrab(object sender, MouseButtonEventArgs eventArgs)
        {
            if (isToolInputBlocked) return;
            if (eventArgs.Source == this || eventArgs.OriginalSource is Line)
            {
                CaptureMouse();
                prevMousePosition = eventArgs.GetPosition(Parent as IInputElement);
                MouseMove += DragOnMouseMove;
                MouseLeftButtonUp += DragStop;
            }
            else if (eventArgs.Source is Vertex vertex)
            {
                vertex.DragStart(eventArgs.GetPosition(vertex));
            }
        }
        private void DragOnMouseMove(object sender, MouseEventArgs eventArgs)
        {
            if (isToolInputBlocked) return;
            var mousePosition = eventArgs.GetPosition(Parent as IInputElement);
            translateTransform.X += mousePosition.X - prevMousePosition.X;
            translateTransform.Y += mousePosition.Y - prevMousePosition.Y;
            prevMousePosition = mousePosition;
        }
        private void DragStop(object sender, MouseEventArgs eventArgs)
        {
            ReleaseMouseCapture();
            MouseMove -= DragOnMouseMove;
            MouseLeftButtonUp -= DragStop;
        }
        public void ResetPosition()
        {
            translateTransform.X = 0;
            translateTransform.Y = 0;
        }
        #endregion

        #region Add & Delete
        private void AddVertex(object sender, MouseButtonEventArgs eventArgs)
        {
            if (isToolInputBlocked) return;
            var clickPosition = eventArgs.GetPosition(this);
            Children.Add(new Vertex(clickPosition.X, clickPosition.Y));
        }
        private void DeleteItem(object sender, MouseButtonEventArgs eventArgs)
        {
            if (isToolInputBlocked) return;
            if (eventArgs.Source is Vertex vertex)
            {
                vertex.Delete();
            }
            else if (eventArgs.Source is Edge edge)
            {
                edge.Delete();
            }
        }
        #endregion

        #region Connect
        private void DirectedEdgeStart(object sender, MouseButtonEventArgs eventArgs)
        {
            if (isToolInputBlocked) return;
            if (eventArgs.Source is Vertex v1)
            {
                MouseLeftButtonDown -= DirectedEdgeStart;
                selectedEdge = new Edge();
                selectedEdge.IsDirected = true;
                ConnectStart(v1);
            }
        }
        private void UndirectedEdgeStart(object sender, MouseButtonEventArgs eventArgs)
        {
            if (isToolInputBlocked) return;
            if (eventArgs.Source is Vertex v1)
            {
                MouseLeftButtonDown -= UndirectedEdgeStart;
                selectedEdge = new Edge();
                selectedEdge.IsDirected = false;
                ConnectStart(v1);
            }
        }
        private void ConnectStart(Vertex v1)
        {
            MouseLeftButtonDown += ConnectEnd;
            MouseMove += ConnectOnMouseMove;

            Children.Add(selectedEdge);
            v1.TryAddEdgeTail(selectedEdge);
        }
        private void ConnectOnMouseMove(object sender, MouseEventArgs eventArgs)
        {
            if (isToolInputBlocked) return;
            var mousePosition = eventArgs.GetPosition(this);
            selectedEdge.FollowMouseX = mousePosition.X;
            selectedEdge.FollowMouseY = mousePosition.Y;
        }
        private void ConnectEnd(object sender, MouseButtonEventArgs eventArgs)
        {
            if (selectedEdge.IsDirected)
            {
                MouseLeftButtonDown += DirectedEdgeStart;
            }
            else
            {
                MouseLeftButtonDown += UndirectedEdgeStart;
            }
            MouseMove -= ConnectOnMouseMove;
            MouseLeftButtonDown -= ConnectEnd;

            if (!(eventArgs.Source is Vertex v2 && v2.TryAddEdgeHead(selectedEdge)) || isToolInputBlocked)
            {
                selectedEdge.Delete();
            }
            selectedEdge = null;
        }
        #endregion

        #region Zoom
        public void Zoom(object sender, MouseWheelEventArgs eventArgs)
        {
            var mousePosition = eventArgs.GetPosition(this);

            if (eventArgs.Delta > 0)
            {
                if (zoomScale > maxZoomScale) return;
                zoomScale++;
                Scale(zoomInFactor, mousePosition);
            }
            else
            {
                if (zoomScale < minZoomScale) return;
                zoomScale--;
                Scale(zoomOutFactor, mousePosition);
            }
        }
        private void Scale(double factor, Point? target = null)
        {
            scaleTransform.ScaleX *= factor;
            scaleTransform.ScaleY *= factor;

            if (target.HasValue)
            {
                translateTransform.X += (((target.Value.X - Width / 2) / factor) + Width / 2 - target.Value.X) * scaleTransform.ScaleX;
                translateTransform.Y += (((target.Value.Y - Height / 2) / factor) + Height / 2 - target.Value.Y) * scaleTransform.ScaleY;
            }
        }
        public void ResetScale()
        {
            zoomScale = 0;
            scaleTransform.ScaleX = 1;
            scaleTransform.ScaleY = 1;
        }
        #endregion

        /// <summary>
        /// To find the shortest path between two vertices, first click on start point, then on end point. Or anywhere else to cancel.
        /// </summary>
        public void ShortestPathPointSelector(object sender, MouseButtonEventArgs eventArgs)
        {
            LinkedList<Edge> result = new LinkedList<Edge>();

            void Cancel()
            {
                isToolInputBlocked = false;
                selectedVertex = null;
                MouseLeftButtonDown -= ShortestPathPointSelector;
            }
            void ResetColors(object s, MouseButtonEventArgs e)
            {
                foreach (Edge edge in result)
                {
                    edge.SetColor(Edge.Colors.Default);
                }
                MouseLeftButtonDown -= ResetColors;
            }


            if (eventArgs.Source is Vertex vertex)
            {
                if (selectedVertex == null)
                {
                    selectedVertex = vertex;
                } 
                else if (selectedVertex != vertex)
                {
                    result = Algorithms.Dijkstra(selectedVertex, vertex, this);
                    if (result == null) MessageBox.Show("No path");
                    else
                    {
                        MessageBox.Show($"Shortest path is {Algorithms.TotalDistance(result)} units long");
                        foreach (Edge edge in result)
                        {
                            edge.SetColor(Edge.Colors.Highlighted);
                        }
                        MouseLeftButtonDown += ResetColors;
                    } 
                    Cancel();
                }
            }
            else
            {
                Cancel();
            }
        }
        
    }
}