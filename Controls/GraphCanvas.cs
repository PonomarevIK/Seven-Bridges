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
        UndoStack undoStack = new UndoStack();
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

        /// <summary>Number of vertices on canvas.</summary>
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

        /// <summary>Get an array of every vertex currently on canvas.</summary>
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
            RemoveElement(selectedEdge);
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
            RemoveElement(selectedEdge);
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

                // Undo
                case Key.Z:
                    UndoAction();
                    break;
            }
        }

        public void BlockToolInput() => isToolInputBlocked = true;
        public void UnblockToolInput() => isToolInputBlocked = false;

        /// <summary>Is called when an edge's weight value is successfuly changed. Adds this change to the undo stack.</summary>
        public void OnEdgeWeightChanged(Edge edge, object prevWeight)
        {
            undoStack.Push(new ChangeEdgeWeight_Action(edge, (float)prevWeight));
        }
        /// <summary>Is called when a vertex's name is successfuly changed. Adds this change to the undo stack.</summary>
        public void OnVertexNameChanged(Vertex vertex, object prevName)
        {
            undoStack.Push(new ChangeVertexName_Action(vertex, (string)prevName));
        }

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
        private void AddChild(UIElement element, bool addToUndoStack = true)
        {
            if (element is Vertex vertex)
            {
                if (addToUndoStack) undoStack.Push(new AddVertex_Action(vertex));
                vertex.NameChanged += OnVertexNameChanged;
            }
            else if (element is Edge edge)
            {
                if (addToUndoStack) undoStack.Push(new AddEdge_Action(edge));
                edge.WeightChanged += OnEdgeWeightChanged;   
            }

            Children.Add(element);
        }

        private void AddVertex(object sender, MouseButtonEventArgs eventArgs)
        {
            if (isToolInputBlocked) return;
            var clickPosition = eventArgs.GetPosition(this);
            AddChild(new Vertex(clickPosition.X, clickPosition.Y));
        }

        private void DeleteItem(object sender, MouseButtonEventArgs eventArgs)
        {
            if (isToolInputBlocked) return;
            if (eventArgs.Source is Vertex vertex)
            {
                undoStack.Push(new DeleteVertex_Action(vertex));
                vertex.NameChanged -= OnVertexNameChanged;
                RemoveElement(vertex);
            }
            else if (eventArgs.Source is Edge edge)
            {
                undoStack.Push(new DeleteEdge_Action(edge));
                edge.WeightChanged -= OnEdgeWeightChanged;
                RemoveElement(edge);
            }
        }

        private void RemoveElement(UIElement element, object parameter=null)
        {
            Children.Remove(element);
            switch (element)
            {
                case Vertex vertex:
                    foreach (Edge edge in vertex.Edges) RemoveElement(edge, vertex);
                    return;

                case Edge edge:
                    if (edge == selectedEdge) undoStack.Pop();
                    edge.Delete(parameter as Vertex);
                    return;
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
                selectedEdge = new Edge() { IsDirected = true };
                ConnectStart(v1);
            }
        }

        private void UndirectedEdgeStart(object sender, MouseButtonEventArgs eventArgs)
        {
            if (isToolInputBlocked) return;
            if (eventArgs.Source is Vertex v1)
            {
                MouseLeftButtonDown -= UndirectedEdgeStart;
                selectedEdge = new Edge() { IsDirected = false };
                ConnectStart(v1);
            }
        }

        private void ConnectStart(Vertex v1)
        {
            MouseLeftButtonDown += ConnectEnd;
            MouseMove += ConnectOnMouseMove;

            AddChild(selectedEdge);
            v1.TryAddEdgeFrom(selectedEdge);
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

            if (!(eventArgs.Source is Vertex v2 && v2.TryAddEdgeTo(selectedEdge)) || isToolInputBlocked)
            {
                RemoveElement(selectedEdge);
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

        public void UndoAction()
        {
            Action action = undoStack.Pop();
            switch (action)
            {
                case AddVertex_Action addVertex_Action:
                    RemoveElement(addVertex_Action.AddedVertex);
                    return;

                case AddEdge_Action addEdge_Action:
                    RemoveElement(addEdge_Action.AddedEdge);
                    return;

                case DeleteVertex_Action deleteVertex_Action:
                    AddChild(deleteVertex_Action.DeletedVertex, false);
                    foreach(var edge in deleteVertex_Action.DeletedVertex.Edges)
                    {
                        AddChild(edge, false);
                        edge.Restore();
                    }
                    return;

                case DeleteEdge_Action deleteEdge_Action:
                    AddChild(deleteEdge_Action.DeletedEdge, false);
                    deleteEdge_Action.DeletedEdge.Restore();
                    return;

                case ChangeEdgeWeight_Action changeEdgeWeight_Action:
                    changeEdgeWeight_Action.Edge.Weight = changeEdgeWeight_Action.PreviousWeight;
                    return;

                case ChangeVertexName_Action changeVertexName_Action:
                    changeVertexName_Action.Vertex.Content = changeVertexName_Action.PreviousName;
                    return;

                default:
                    return;
            }
        }

        /// <summary>To find the shortest path between two vertices, first click on start point, then on end point. Or anywhere else to cancel.</summary>
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