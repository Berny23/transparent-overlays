using System.Windows;
using System.Windows.Input;

namespace WPF_OverlayTransparent
{
    public partial class Dialog : Window
    {
        public Dialog()
        {
            InitializeComponent();

            ResponseTextBox.Focus();
        }

        public string ResponseText
        {
            get { return ResponseTextBox.Text; }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }
    }
}
