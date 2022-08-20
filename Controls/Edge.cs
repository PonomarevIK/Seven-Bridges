using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
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

        private double? x2;
        public double X2
        {
            get => x2 ?? V1.CenterX;
            set
            {
                x2 = value;
                OnPropertyChanged("X2");
            }
        }
        private double? y2;
        public double Y2
        {
            get => y2 ?? V1.CenterY;
            set
            {
                y2 = value;
                OnPropertyChanged("Y2");
            }
        }

        public bool IsDirected = false; // TODO
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

        public void Delete()
        {
            v1?.Edges.Remove(this);
            v2?.Edges.Remove(this);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
