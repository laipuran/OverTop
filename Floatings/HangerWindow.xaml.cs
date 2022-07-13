using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace OverTop.Floatings
{
    /// <summary>
    /// HangerWindow.xaml 的交互逻辑
    /// OKButton 是编程加入的
    /// AddTextButton 是 HangerWindow.xaml 中 ContextMenu 的按钮
    /// </summary>
    public partial class HangerWindow : Window
    {
        public HangerWindow()
        {
            InitializeComponent();
            Width = Settings.Default.width;
            Height = Settings.Default.height;
            App.parameterClass.backGroundColor = Color.FromRgb(Settings.Default.colorR, Settings.Default.colorG, Settings.Default.colorB);
            Background = new SolidColorBrush(App.parameterClass.backGroundColor);

            if (App.parameterClass.alpha != 0)
                Opacity = App.parameterClass.alpha;
            else
                Opacity = 0.8;

        }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (System.Windows.Input.Mouse.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                Close();
            }
        }

        private void ContentStackPanel_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Scroller.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
        }

        private void Window_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Scroller.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
        }

        private void Window_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            App.currentWindow = this;
            App.contentStackPanel = ContentStackPanel;
            Window propertyWindow = new PropertyWindow
            {
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            propertyWindow.ShowDialog();
            Width = App.parameterClass.width;
            Height = App.parameterClass.height;
            Background = new SolidColorBrush(App.parameterClass.backGroundColor);
            Opacity = App.parameterClass.alpha;
        }
    }
}
