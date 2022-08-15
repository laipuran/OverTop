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
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using static OverTop.AppWindowClass;

namespace OverTop.Floatings
{
    /// <summary>
    /// AppWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AppWindow : Window
    {
        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        private static bool isBottom = false;
        private static string filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\OverTop\\AppWindow.json";
        public static Dictionary<string, StackPanel> controls = new();
        
        public AppWindow()
        {
            InitializeComponent();
            LoadIcons();
        }

        private void AppPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
#pragma warning disable CS8604 // 引用类型参数可能为 null。
            Process.Start("explorer.exe", ((StackPanel)sender).ToolTip.ToString());
#pragma warning restore CS8604 // 引用类型参数可能为 null。
        }
        
        private async void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.C))
            {
                ContentStackPanel.Children.Clear();
                return;
            }
            else if (Keyboard.IsKeyDown(Key.R))
            {
                ContentStackPanel.Children.Remove((UIElement)sender);
                return;
            }
            DragMove();
            await Task.Run(() => Dispatcher.BeginInvoke(new Action(TrySetPosition)));
        }

        private void Window_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Window chooserWindow = new ChooserWindow();
            App.contentStackPanel = ContentStackPanel;
            chooserWindow.ShowDialog();
        }

        private void SetWindowPos(ScreenPart part)
        {
            if (part == ScreenPart.TopPart)
            {
                Height = 60;
                Width = 560;
                ContentStackPanel.Orientation = System.Windows.Controls.Orientation.Horizontal;
                Top = 0;
                Left = (SystemParameters.FullPrimaryScreenWidth - Width) / 2;
            }
            else if (part == ScreenPart.BottomPart)
            {
                Height = 60;
                Width = 560;
                ContentStackPanel.Orientation = System.Windows.Controls.Orientation.Horizontal;
                Top = SystemParameters.FullPrimaryScreenHeight - Height;
                Left = (SystemParameters.FullPrimaryScreenWidth - Width) / 2;
            }
            else if (part == ScreenPart.LeftPart)
            {
                Height = 560;
                Width = 60;
                ContentStackPanel.Orientation = System.Windows.Controls.Orientation.Vertical;
                Top = (SystemParameters.FullPrimaryScreenHeight - Height) / 2;
                Left = 0;
            }
            else
            {
                Height = 560;
                Width = 60;
                ContentStackPanel.Orientation = System.Windows.Controls.Orientation.Vertical;
                Top = (SystemParameters.FullPrimaryScreenHeight - Height) / 2;
                Left = SystemParameters.FullPrimaryScreenWidth - Width;
            }
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
            ScreenPart part = GetPart(GetMiddlePoint(this));
            SetWindowPos(part);
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
#pragma warning disable CS8602 // 解引用可能出现空引用。
            App.mainWindow.Visibility = Visibility.Visible;
            App.mainWindow.Activate();
#pragma warning restore CS8602 // 解引用可能出现空引用。
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Tab)
            {
                isBottom = WindowClass.ChangeStatus(isBottom, this);
            }
        }

        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            WindowClass.ChangeZIndex(isBottom, this);
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            WindowClass.ChangeZIndex(isBottom, this);
        }

        private void LoadIcons()
        {
            if (!File.Exists(filePath))
            {
                SetWindowPos(ScreenPart.TopPart);
                return;
            }
            string json = File.ReadAllText(filePath);

#pragma warning disable CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
            AppWindowClass appWindowClass = JsonConvert.DeserializeObject<AppWindowClass>(json);
#pragma warning restore CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
#pragma warning disable CS8602 // 解引用可能出现空引用。
            foreach (string item in appWindowClass.filePath)
#pragma warning restore CS8602 // 解引用可能出现空引用。
            {
                try
                {
                    StackPanel appPanel = new();
                    System.Windows.Controls.Image image = new();
#pragma warning disable CS8602 // 解引用可能出现空引用。
                    Bitmap icon = System.Drawing.Icon.ExtractAssociatedIcon(item).ToBitmap();
#pragma warning restore CS8602 // 解引用可能出现空引用。
                    image.Source = System.Windows.Interop.Imaging
                        .CreateBitmapSourceFromHBitmap(icon.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    image.Width = 40;
                    Thickness margin = new(10, 10, 10, 10);
                    appPanel.Children.Add(image);
                    appPanel.Margin = margin;
                    appPanel.ToolTip = item;
                    appPanel.MouseLeftButtonDown += AppPanel_MouseLeftButtonDown;
                    appPanel.AllowDrop = true;
                    appPanel.Drop += AppPanel_Drop;
                    controls.Add(item, appPanel);
                    ContentStackPanel.Children.Add(appPanel);
                }
                catch { }
            }
            SetWindowPos(appWindowClass.screenPart);
        }

        private void AppPanel_Drop(object sender, DragEventArgs e)
        {
            string path = Path.GetFileNameWithoutExtension(((System.Windows.Controls.Image)((StackPanel)sender).Children[0]).ToolTip.ToString());
            Process[] processes = Process.GetProcessesByName(path);
            foreach (Process item in processes)
            {
                IntPtr hWnd = item.Handle;
                ShowWindowAsync(hWnd, 5);
            }
        }
    }
}
