using System.Windows;
using System.Windows.Input;

namespace OverTop.Floatings
{
    /// <summary>
    /// RecentWindow.xaml 的交互逻辑
    /// </summary>
    public partial class RecentWindow : Window
    {
        public RecentWindow()
        {
            InitializeComponent();
        }
        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
