using System.Windows;
using System.Windows.Input;

namespace Seven_Bridges
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void InitializeWindow(object sender, RoutedEventArgs eventArgs)
        {
            MyCanvas.Initialize();
            AddToolRadioButton.IsChecked = true;
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


        private void KeyCommand(object sender, KeyEventArgs eventArgs)
        {
            if (eventArgs.Source != MyCanvas) return;
            switch (eventArgs.Key)
            {
                case Key.D1:
                case Key.NumPad1:
                    DragToolRadioButton.IsChecked = true;
                    break;
                case Key.D2:
                case Key.NumPad2:
                    AddToolRadioButton.IsChecked = true;
                    break;
                case Key.D3:
                case Key.NumPad3:
                    DeleteToolRadioButton.IsChecked = true;
                    break;
                case Key.D4:
                case Key.NumPad4:
                    UndirectedEdgeToolRadioButton.IsChecked = true;
                    break;
                case Key.D5:
                case Key.NumPad5:
                    DirectedEdgeToolRadioButton.IsChecked = true;
                    break;
            }
        }


        private void ShowComponentCount(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"Components: {Algorithms.ComponentCount(MyCanvas)}");
        }

        private void ShortestPathCalled(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("First click on starting vertex, then on destination. To cancel the opertation, click anywhere else.");
            MyCanvas.ShortestPathSelected();
        }
    }
}
