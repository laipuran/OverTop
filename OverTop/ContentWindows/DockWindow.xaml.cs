using OverTop.FunctionalWindows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static OverTop.AppWindowOps;
using static PuranLai.Tools.ExtendedWindowOps;

namespace OverTop.ContentWindows
{
    /// <summary>
    /// AppWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DockWindow : Window
    {
        public bool isTop = true;
        public bool isMouseIn = false;
        public StackPanel ContentPanel;
        public AppWindowProperty Property;
        string currentWindowName = "";
        List<WindowInfo> LatestWindows = new();
        public DockWindow(AppWindowProperty property)
        {
            InitializeComponent();
            Property = property;
            ContentPanel = ContentStackPanel;
        }

        public DockWindow()
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
            SelectorWindow chooserWindow = new SelectorWindow(this);
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
            this.Reload();
        }

        [DllImport("user32")]
        private static extern void SetForegroundWindow(IntPtr hWnd);

        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            IReadOnlyList<WindowInfo> windows = WindowEnumerator.FindAll();
            List<WindowInfo> WindowList = new(), temp = new();
            temp.AddRange(windows);
            foreach (var item in windows)
            {
                if (item.Title != "DockPanel"
                    && item.Title != "Microsoft Text Input Application"
                    && item.Title != "Program Manager")
                {
                    WindowList.Add(item);
                }
            }
            bool flag = false;
            if (LatestWindows.Count != WindowList.Count)
            {
                flag = true;
            }
            foreach (var item in WindowList)
            {
                if (!LatestWindows.Contains(item))
                {
                    flag = true;
                }
            }
            if (flag)
            {
                LatestWindows = new();
                LatestWindows.AddRange(WindowList);
            }
            else
            {
                WindowList = LatestWindows;
            }

            if (String.IsNullOrEmpty(currentWindowName))
            {
                WindowInfo first = WindowList[0];
                currentWindowName = first.Title;
                SetForegroundWindow(first.Hwnd);
                return;
            }

            bool up = e.Delta > 0 ? true : false;
            foreach (var WindowInfo in windows)
            {
                if (WindowInfo.Title == currentWindowName)
                {
                    int index = WindowList.IndexOf(WindowInfo);
                    index = up ? index + 1 : index - 1;

                    while (true)
                    {
                        if (index >= WindowList.Count)
                        {
                            index = 0;
                        }
                        else if (index < 0)
                        {
                            index = WindowList.Count - 1;
                        }
                        else
                            break;
                    }

                    WindowInfo next = WindowList[index];
                    currentWindowName = next.Title;
                    SetForegroundWindow(next.Hwnd);
                    return;
                }
            }
            WindowInfo info = WindowList[0];
            currentWindowName = info.Title;
            SetForegroundWindow(info.Hwnd);
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string filePath in files)
                {
                    App.DockPanel.Property.AddApplication(filePath);
                }
            }
        }
    }
}
