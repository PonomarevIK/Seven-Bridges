using System.Windows;
using System.Windows.Input;

namespace Seven_Bridges
{
    public partial class TextPrompt : Window
    {
        public TextPrompt(string prompt)
        {
            InitializeComponent();
            Prompt.Text = prompt;
            Response.Focus();
        }

        public string ResponseText
        {
            get => Response.Text;
            set => Response.Text = value;
        }

        private void OKButton(object sender, RoutedEventArgs eventArgs)
        {
            DialogResult = true;
        }

        private void StackPanel_KeyDown(object sender, KeyEventArgs eventArgs)
        {
            switch (eventArgs.Key)
            {
                case Key.Enter:
                    DialogResult = true;
                    break;
                case Key.Escape:
                    DialogResult = false;
                    break;
            }
        }

        private void CancelButton(object sender, RoutedEventArgs eventArgs)
        {
            DialogResult = false;
        }
    }
}
