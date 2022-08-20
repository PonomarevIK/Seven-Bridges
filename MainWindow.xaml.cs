﻿using System;
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
            string output;
            void WhatType()
            {
                var src = eventArgs.Source.GetType();
                var org = eventArgs.OriginalSource.GetType();
                var snd = sender.GetType();
                output = $" Source: {src}\nOriginal: {org}\nSender: {snd}";
            }
            void ElementPosition()
            {
                var element = (UIElement)sender;
                output = $"{GraphCanvas.GetLeft(element)} ; {GraphCanvas.GetTop(element)}";
            }
            void MousePosition()
            {
                var cursor = eventArgs.GetPosition(sender as GraphCanvas);
                output = $"{(int)cursor.X} ; {(int)cursor.Y}";
            }

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

        private void ConnectToolChecked(object sender, RoutedEventArgs eventArgs) => MyCanvas.ConnectToolSelected();
        private void ConnectToolUnchecked(object sender, RoutedEventArgs eventArgs) => MyCanvas.ConnectToolUnselected();
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
            r2.IsChecked = true;
        }
    }

    public class AverageValue : IMultiValueConverter
    {
        private static Func<object, double> propertyConverter = (value) => value == DependencyProperty.UnsetValue ? 0 : System.Convert.ToDouble(value);

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return values.Average(propertyConverter);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CenteredMargin : IMultiValueConverter
    {
        private static Func<object, bool> isSet = (value) => value != DependencyProperty.UnsetValue;

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.All(isSet))
            {
                return new Thickness(-System.Convert.ToDouble(values[0]) / 2.0, -System.Convert.ToDouble(values[1]) / 2.0, 0, 0);
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}