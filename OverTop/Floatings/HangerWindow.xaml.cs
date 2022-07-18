using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;

namespace OverTop.Floatings
{
    /// <summary>
    /// HangerWindow.xaml 的交互逻辑
    /// </summary>
    public partial class HangerWindow : Window
    {
        public const int HWND_TOP = 0;
        public const int HWND_BOTTOM = 1;
        public const int HWND_TOPMOST = -1;
        public const int HWND_NOTOPMOST = -2;
        IntPtr hWnd = new();
        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndlnsertAfter, int X, int Y, int cx, int cy, uint Flags);

        private static bool isChild = false;
        public HangerWindow()
        {
            InitializeComponent();
            Width = HangerSettings.Default.width;
            Height = HangerSettings.Default.height;
            System.Windows.Media.Color color = System.Windows.Media.Color.FromRgb(HangerSettings.Default.color.R, HangerSettings.Default.color.G, HangerSettings.Default.color.B);
            Background = new SolidColorBrush(color);
            Opacity = HangerSettings.Default.alpha == 0.0 ? 0.8 : HangerSettings.Default.alpha;
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
            else if (e.Key == System.Windows.Input.Key.Tab)
            {
                if (!isChild)
                {
                    isChild = true;
                    Topmost = false;
                    ToBottom();
                }
                else
                {
                    isChild = false;
                    Topmost = true;
                    ToTop();
                }
            }
        }
        private void ToBottom()
        {
            hWnd = new WindowInteropHelper(this).Handle;
            SetWindowPos(hWnd, (IntPtr)HWND_BOTTOM, (int)Left, (int)Top, (int)Width, (int)Height, 0);
        }
        private void ToTop()
        {
            hWnd = new WindowInteropHelper(this).Handle;
            SetWindowPos(hWnd, (IntPtr)HWND_TOPMOST, (int)Left, (int)Top, (int)Width, (int)Height, 0);
        }
        private void ContentStackPanel_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (isChild)
            {
                ToBottom();
            }
            DragMove();
        }

        private void Window_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (isChild)
            {
                ToBottom();
            }
            Scroller.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
        }

        private void Window_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (isChild)
            {
                ToBottom();
            }
            Scroller.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
        }

        private void Window_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            App.currentWindow = this;
            App.contentStackPanel = ContentStackPanel;
            App.windowType = App.WindowType.Hanger;
            Window propertyWindow = new PropertyWindow
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            propertyWindow.ShowDialog();
            Width = App.parameterClass.width;
            Height = App.parameterClass.height;
            Background = new SolidColorBrush(App.parameterClass.backGroundColor);
            Opacity = App.parameterClass.alpha;
        }
    }
}
