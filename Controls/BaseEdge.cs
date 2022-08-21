using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace Seven_Bridges.Controls
{
    public abstract class BaseEdge : UserControl, INotifyPropertyChanged
    {
        protected Vertex v1;
        public abstract Vertex V1 { get; set; }
        protected Vertex v2;
        public abstract  Vertex V2 { get; set; }

        protected double? followMouseX;
        public abstract double FollowMouseX { get; set; }
        protected double? followMouseY;
        public abstract double FollowMouseY { get; set; }

        protected int? weight;
        public abstract int? Weight { get; set; }
        public abstract Visibility WeightVisibility { get; }

        protected void OnHover(object sender, MouseEventArgs eventArgs)
        {
            OnPropertyChanged("WeightVisibility");
        }

        public abstract void Delete(Vertex sourceVertex = null);

        public abstract event PropertyChangedEventHandler PropertyChanged;
        protected abstract void OnPropertyChanged(string propertyName);
    }
}
