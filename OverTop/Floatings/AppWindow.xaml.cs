using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static OverTop.AppWindowOps;
using static PuranLai.Tools.ExtendedWindowOps;

namespace OverTop.Floatings
{
    /// <summary>
    /// AppWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AppWindow : Window
    {
        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
        private static bool isTop = true;

        public bool isMouseIn = false;
        public StackPanel ContentPanel;
        public AppWindowProperty Property;

        public AppWindow(AppWindowProperty property)
        {
            InitializeComponent();
            Property = property;
            ContentPanel = ContentStackPanel;
        }

        public AppWindow()
        {
            ContentPanel = new();
            Property = new();
        }

        private async void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.C))
            {
                ContentStackPanel.Children.Clear();
                return;
            }
            else if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
                await Task.Run(() => Dispatcher.BeginInvoke(new Action(SetSide)));
            }
        }

        private void Window_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Window chooserWindow = new ChooserWindow(this);
            try
            {
                chooserWindow.Show();
            }
            catch { }
        }

        private void SetSide()
        {
            while (true)
            {
                if (Mouse.LeftButton == MouseButtonState.Released)
                {
                    break;
                }
            }
            ScreenPart part = this.GetMiddlePoint().GetPart();
            this.SetWindowPos(part);
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                App.MainWindow.WindowState = WindowState.Normal;
                App.MainWindow.Opacity = 1;
                App.MainWindow.Visibility = Visibility.Visible;
                App.MainWindow.Activate();
                App.MainWindow.Focus();
            }
            catch { }
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Tab)
            {
                isTop = CommonWindowOps.ChangeStatus(isTop, this);
            }
        }

        private unsafe void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            CommonWindowOps.ChangeZIndex(isTop, this);
            fixed (bool* MouseIn = &isMouseIn)
            {
                this.ChangeOpacity(OpacityOptions._75, MouseIn);
            }
        }

        private unsafe void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            CommonWindowOps.ChangeZIndex(isTop, this);
            fixed (bool* MouseIn = &isMouseIn)
            {
                this.ChangeOpacity(OpacityOptions._25, MouseIn);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Property.ReloadWindow(this);
        }
    }
}
