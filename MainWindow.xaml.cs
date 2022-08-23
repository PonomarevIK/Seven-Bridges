using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Diagnostics;
using Seven_Bridges.Controls;

namespace Seven_Bridges
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Debug(object sender, MouseButtonEventArgs eventArgs)
        {
            //string output;
            //void WhatType()
            //{
            //    var src = eventArgs.Source.GetType();
            //    var org = eventArgs.OriginalSource.GetType();
            //    var snd = sender.GetType();
            //    output = $" Source: {src}\nOriginal: {org}\nSender: {snd}";
            //}
            //void ElementPosition()
            //{
            //    var element = (UIElement)sender;
            //    output = $"{GraphCanvas.GetLeft(element)} ; {GraphCanvas.GetTop(element)}";
            //}
            //void MousePosition()
            //{
            //    var cursor = eventArgs.GetPosition(sender as GraphCanvas);
            //    output = $"{(int)cursor.X} ; {(int)cursor.Y}";
            //}
            //void GetWidth()
            //{
            //    output = (sender as TextBlock).ActualWidth.ToString();
            //}

            //MessageBox.Show(output);
            //Console.WriteLine(output);
        }

        #region Tool Panel Events
        private void DragToolChecked(object sender, RoutedEventArgs eventArgs) => MyCanvas.DragToolSelected();
        private void DragToolUnchecked(object sender, RoutedEventArgs eventArgs) => MyCanvas.DragToolUnselected();

        private void AddToolChecked(object sender, RoutedEventArgs eventArgs) => MyCanvas.AddToolSelected();
        private void AddToolUnchecked(object sender, RoutedEventArgs eventArgs) => MyCanvas.AddToolUnselected();

        private void DeleteToolChecked(object sender, RoutedEventArgs eventArgs) => MyCanvas.DeleteToolSelected();
        private void DeleteToolUnchecked(object sender, RoutedEventArgs eventArgs) => MyCanvas.DeleteToolUnselected();

        private void UndirectedEdgeToolChecked(object sender, RoutedEventArgs eventArgs) => MyCanvas.UndirectedEdgeToolSelected();
        private void UndirectedEdgeToolUnchecked(object sender, RoutedEventArgs eventArgs) => MyCanvas.UndirectedEdgeToolUnselected();

        private void DirectedEdgeToolChecked(object sender, RoutedEventArgs eventArgs) => MyCanvas.DirectedEdgeToolSelected();
        private void DirectedEdgeToolUnchecked(object sender, RoutedEventArgs eventArgs) => MyCanvas.DirectedEdgeToolUnselected();
        #endregion


        private void KeyCommand(object sender, KeyEventArgs e)
        {
            //switch (e.Key)
            //{
            //    case Key.Escape:
            //        Close();
            //        break;
            //    case Key.D1:
            //    case Key.NumPad1:
            //        r1.IsChecked = true;
            //        break;
            //    case Key.D2:
            //    case Key.NumPad2:
            //        r2.IsChecked = true;
            //        break;
            //    case Key.D3:
            //    case Key.NumPad3:
            //        r3.IsChecked = true;
            //        break;
            //}
        }

        private void OnCanvasLoaded(object sender, RoutedEventArgs e)
        {
            var canvas = (GraphCanvas)sender;
            canvas.Initialize();
            AddToolRadioButton.IsChecked = true;
        }
    }
}
