using System;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using System.Globalization;
using System.Windows.Media;

namespace Seven_Bridges.Controls
{
    public class Edge : UserControl, INotifyPropertyChanged
    {
        public enum Colors
        {
            Default, 
            Highlighted,
        }

        // Tail vertex
        private Vertex v1;
        public Vertex V1
        {
            get => v1;
            set
            {
                v1 = value;
                OnPropertyChanged("V1");
                OnPropertyChanged("WeightVisibility");
            }
        }
        // Head vertex
        private Vertex v2;
        public Vertex V2
        {
            get => v2;
            set
            {
                v2 = value;
                OnPropertyChanged("V2");
                OnPropertyChanged("WeightVisibility");
            }
        }

        // Edge endpoint follows the mouse cursor until it is connected to a vertex
        private double? followMouseX;
        public double FollowMouseX
        {
            get => followMouseX ?? V1.CenterX;
            set
            {
                followMouseX = value;
                OnPropertyChanged("FollowMouseX");
            }
        }
        private double? followMouseY;
        public double FollowMouseY
        {
            get => followMouseY ?? V1.CenterY;
            set
            {
                followMouseY = value;
                OnPropertyChanged("FollowMouseY");
            }
        }

        private float? weight;
        public float Weight
        {
            get => weight ?? 1;
            set
            {
                weight = value;
                OnPropertyChanged("Weight");
                OnPropertyChanged("WeightStr");
                OnPropertyChanged("WeightVisibility");
            }
        }
        public string WeightStr
        {
            get => Weight.ToString();
            set
            {
                try
                {
                    Weight = Convert.ToSingle(value, CultureInfo.InvariantCulture);
                }
                catch (FormatException)
                {
                    Weight = 1;
                }
            }
        }
        // Weight should be displayed only if it's not at default value (1) or if there is a mouse cursor hovering over it
        public Visibility WeightVisibility
        {
            get => (v1 != null && v2 != null && (Weight != 1 || IsMouseOver)) ? Visibility.Visible : Visibility.Hidden;
        }

        private Brush color;
        public Brush Color
        {
            get => color ?? Brushes.Black;
            set
            {
                color = value;
                OnPropertyChanged("Color");
            }
        }
        public void SetColor(Colors color)
        {
            switch (color)
            {
                case Colors.Highlighted:
                    Color = Brushes.Red;
                    break;
                case Colors.Default:
                default:
                    Color = Brushes.Black;
                    break;
            }

        }

        private bool isDirected;
        public bool IsDirected
        {
            get => isDirected;
            set
            {
                isDirected = value;
                OnPropertyChanged("IsDirected");
            }
        }
        
        static Edge()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Edge), new FrameworkPropertyMetadata(typeof(Edge)));
        }
        public Edge()
        {
            DataContext = this;
            Panel.SetZIndex(this, -1);
            MouseEnter += OnHover;
            MouseLeave += OnHover;
            MouseLeave += Unfocus;
            PreviewTextInput += ValidateWeightInput;
        }
        public Edge(Vertex v1, Vertex v2, float weight, bool isDirected) : this()
        {
            V1 = v1;
            V2 = v2;
            Weight = weight;
            IsDirected = isDirected;
        }

        private void OnHover(object sender, MouseEventArgs eventArgs)
        {
            OnPropertyChanged("WeightVisibility");
        }

        public void Unfocus(object sender, MouseEventArgs eventArgs)
        {
            Keyboard.Focus(Parent as IInputElement);
        }

        public void ValidateWeightInput(object sender, TextCompositionEventArgs eventArgs)
        {
            if (!eventArgs.Text.All(Char.IsDigit) && eventArgs.Text != ".")
            {
                eventArgs.Handled = true;
            }
        }

        public void Delete(Vertex sourceVertex = null)
        {
            if (v1 != sourceVertex)
            {
                v1?.Edges.Remove(this);
            }
            if (v2 != sourceVertex)
            {
                v2?.Edges.Remove(this);
            }
        }
        public void Restore()
        {
            this.V1.TryAddEdgeFrom(this);
            this.V2.TryAddEdgeTo(this);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
