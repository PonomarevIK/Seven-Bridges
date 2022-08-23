using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace Seven_Bridges.Controls
{
    public class Edge : UserControl, INotifyPropertyChanged
    {
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

        private int? weight;
        public int? Weight
        {
            get => weight;
            set
            {
                weight = value;
                OnPropertyChanged("Weight");
                OnPropertyChanged("WeightVisibility");
            }
        }
        public Visibility WeightVisibility
        {
            get => (v1 != null && v2 != null && (weight.HasValue || IsMouseOver)) ? Visibility.Visible : Visibility.Hidden;
            //get => Visibility.Hidden;
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

        public Edge()
        {
            DataContext = this;
            Panel.SetZIndex(this, -1);
            MouseEnter += OnHover;
            MouseLeave += OnHover;
        }

        private void OnHover(object sender, MouseEventArgs eventArgs)
        {
            OnPropertyChanged("WeightVisibility");
        }

        public void Delete(Vertex sourceVertex = null)
        {
            if (v1 != null && v1 != sourceVertex)
            {
                v1.Edges.Remove(this);
                v1 = null;
            }
            if (v2 != null && v2 != sourceVertex)
            {
                v2.Edges.Remove(this);
                v2 = null;
            }
            (Parent as Panel).Children.Remove(this);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
