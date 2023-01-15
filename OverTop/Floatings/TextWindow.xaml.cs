using System.Windows;

namespace OverTop.Floatings
{
    /// <summary>
    /// TextWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TextWindow : Window
    {
        public string? result;
        public TextWindow(string tip, string? input)
        {
            InitializeComponent();
            if (input is not null)
                ContentTextBox.Text = input;
            TipTextBlock.Text = tip;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            result = ContentTextBox.Text;
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ContentTextBox.Focus();
        }

        private void Window_LostFocus(object sender, RoutedEventArgs e)
        {
            result = ContentTextBox.Text;
            try
            {
                this.Close();
            }
            catch { }
        }
    }
}
