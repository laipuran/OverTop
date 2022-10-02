using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using static OverTop.CommonWindowOps;
using static OverTop.HangerWindowOps;

namespace OverTop.Floatings
{
    /// <summary>
    /// HangerWindow.xaml 的交互逻辑
    /// </summary>
    public partial class HangerWindow : Window
    {
        private static bool isBottom = false;

        public HangerWindow()
        {
            InitializeComponent();
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
            if (e.Key == System.Windows.Input.Key.Tab)
            {
                isBottom = CommonWindowOps.ChangeStatus(isBottom, this);
                return;
            }
            try
            {
                switch (e.Key)
                {
                    case Key.Left: Left -= 1; break;
                    case Key.Right: Left += 1; break;
                    case Key.Up: Top -= 1; break;
                    case Key.Down: Top += 1; break;
                    case Key.Escape: Close(); break;
                    default: break;
                }
            }
            catch { }
        }

        private void ContentStackPanel_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.LeftCtrl))
            {
                this.Save();
            }
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Window_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            CommonWindowOps.ChangeZIndex(isBottom, this);
            Scroller.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
        }

        private void Window_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            CommonWindowOps.ChangeZIndex(isBottom, this);
            Scroller.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
        }

        private void Window_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            App.currentWindow = this;
            App.contentStackPanel = ContentStackPanel;
            App.currentWindowType = CommonWindowOps.WindowType.Hanger;
            Window propertyWindow = new PropertyWindow
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            propertyWindow.ShowDialog();
            Width = App.tempProperty.width;
            Height = App.tempProperty.height;
            Background = new SolidColorBrush(App.tempProperty.backGroundColor);
            Opacity = App.tempProperty.alpha;
        }

    }
}
