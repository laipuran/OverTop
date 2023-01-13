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
using System.Windows.Shapes;

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
            this.Close();
        }
    }
}
