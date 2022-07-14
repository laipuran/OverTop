using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
            Width = Settings.Default.width;
            Height = Settings.Default.height;
            Color color = Color.FromRgb(Settings.Default.color.R, Settings.Default.color.G, Settings.Default.color.B);
            Background = new SolidColorBrush(color);
            Opacity = Settings.Default.alpha == 0.0 ? 0.8 : Settings.Default.alpha;
        }
        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            App.currentWindow = this;
            App.windowType = App.WindowType.Recent;
            Window propertyWindow = new PropertyWindow
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            propertyWindow.ShowDialog();
            Width = App.parameterClass.width;
            Height = App.parameterClass.height;
            Background = new SolidColorBrush(App.parameterClass.backGroundColor);
            Opacity = App.parameterClass.alpha;
            //TODO: Change 1 to a proper value
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            Scroller.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            Scroller.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        private void ContentStackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
