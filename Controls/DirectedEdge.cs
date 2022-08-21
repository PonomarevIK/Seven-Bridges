using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace Seven_Bridges.Controls
{
    public class DirectedEdge : BaseEdge
    {
        public override Vertex V1
        {
            get => v1;
            set
            {
                v1 = value;
                OnPropertyChanged("V1");
                OnPropertyChanged("WeightVisibility");
            }
        }
        public override Vertex V2
        {
            get => v2;
            set
            {
                v2 = value;
                OnPropertyChanged("V2");
                OnPropertyChanged("WeightVisibility");
            }
        }

        public override double FollowMouseX
        {
            get => followMouseX ?? V1.CenterX;
            set
            {
                followMouseX = value;
                OnPropertyChanged("FollowMouseX");
            }
        }
        public override double FollowMouseY
        {
            get => followMouseY ?? V1.CenterY;
            set
            {
                followMouseY = value;
                OnPropertyChanged("FollowMouseY");
            }
        }

        public override int? Weight
        {
            get => weight;
            set
            {
                weight = value;
                OnPropertyChanged("Weight");
                OnPropertyChanged("WeightVisibility");
            }
        }
        public override Visibility WeightVisibility
        {
            get => (v1 != null && v2 != null && (weight.HasValue || IsMouseOver)) ? Visibility.Visible : Visibility.Hidden;
            //get => Visibility.Hidden;
        }

        public DirectedEdge()
        {
            DataContext = this;
            Panel.SetZIndex(this, -1);
            MouseEnter += OnHover;
            MouseLeave += OnHover;
        }

        public override void Delete(Vertex sourceVertex = null)
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

        public override event PropertyChangedEventHandler PropertyChanged;
        protected override void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
