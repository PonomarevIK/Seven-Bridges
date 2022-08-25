using System;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Data;
using System.Linq;
using System.Globalization;

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
        public Visibility WeightVisibility
        {
            get => (v1 != null && v2 != null && (Weight != 1 || IsMouseOver)) ? Visibility.Visible : Visibility.Hidden;
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
            MouseLeave += Unfocus;
            PreviewTextInput += ValidateWeightInput;
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
