using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static OverTop.AppWindowOps;
using static OverTop.CommonWindowOps;
using static PuranLai.Tools.ExtendedWindowOps;
using Color = System.Windows.Media.Color;

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
                await Task.Run(() => Dispatcher.BeginInvoke(new Action(TrySetPosition)));
            }
        }

        private void Window_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Window chooserWindow = new ChooserWindow();
            try
            {
                chooserWindow.ShowDialog();
            }
            catch { }
        }

        private void TrySetPosition()
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
#pragma warning disable CS8602 // 解引用可能出现空引用。
            try
            {
                App.MainWindow.WindowState = WindowState.Normal;
                App.MainWindow.Opacity = 1;
                App.MainWindow.Visibility = Visibility.Visible;
                App.MainWindow.Activate();
                App.MainWindow.Focus();
            }
            catch { }
#pragma warning restore CS8602 // 解引用可能出现空引用。
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
