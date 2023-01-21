using Newtonsoft.Json;
using PuranLai.Tools.Classes;
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
using System.Windows.Media.Imaging;
using static OverTop.AppWindowOps;
using static OverTop.CommonWindowOps;

namespace OverTop.Floatings
{
    /// <summary>
    /// AppWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AppWindow : Window
    {
        public bool isMouseIn = false;
        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        private static bool isBottom = false;
        private static string filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\OverTop\\AppWindow.json";

        public static Dictionary<string, StackPanel> controls = new();

        public AppWindow()
        {
            InitializeComponent();
        }

        public static void AppPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
#pragma warning disable CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
            string path = ((StackPanel)sender).ToolTip.ToString();
#pragma warning restore CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
            if (Keyboard.IsKeyDown(Key.R) || !File.Exists(path))
            {
#pragma warning disable CS8604 // 引用类型参数可能为 null。
                controls.Remove(path);
                ((StackPanel)((StackPanel)sender).Parent).Children.Remove(sender as UIElement);
            }
            else if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
#pragma warning disable CS8602 // 解引用可能出现空引用。
                Process.Start("explorer.exe", Directory.GetParent(path).ToString());
#pragma warning restore CS8602 // 解引用可能出现空引用。
            }
            else
                Process.Start("explorer.exe", path);
#pragma warning restore CS8604 // 引用类型参数可能为 null。
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
            App.contentStackPanel = ContentStackPanel;
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
                App.mainWindow.Visibility = Visibility.Visible;
                App.mainWindow.Activate();

                App.mainWindow.Height = 450;
                App.mainWindow.Width = 750;
                App.mainWindow.Opacity = 1;
                App.mainWindow.Left = (SystemParameters.PrimaryScreenWidth - 750) / 2;
                App.mainWindow.Top = (SystemParameters.PrimaryScreenHeight - 450) / 2;
                App.mainWindow.Focus();
            }
            catch { }
#pragma warning restore CS8602 // 解引用可能出现空引用。
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Tab)
            {
                isBottom = CommonWindowOps.ChangeStatus(isBottom, this);
            }
        }

        private unsafe void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            CommonWindowOps.ChangeZIndex(isBottom, this);
            fixed (bool* MouseIn = &isMouseIn)
            {
                WindowOperations.ToWhite(MouseIn, this);
            }
        }

        private unsafe void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            CommonWindowOps.ChangeZIndex(isBottom, this);
            fixed (bool* MouseIn = &isMouseIn)
            {
                WindowOperations.ToTransparent(MouseIn, this);
            }
        }

        private void LoadFilesFromString(string filePath)
        {
            if (!File.Exists(filePath))
            {
                this.SetWindowPos(ScreenPart.TopPart);
                return;
            }
            string json = File.ReadAllText(filePath);

#pragma warning disable CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
            AppWindowOps appWindowClass = JsonConvert.DeserializeObject<AppWindowOps>(json);
#pragma warning restore CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
#pragma warning disable CS8602 // 解引用可能出现空引用。
            foreach (string item in appWindowClass.filePath)
#pragma warning restore CS8602 // 解引用可能出现空引用。
            {
                try
                {
                    System.Windows.Controls.Image image = new();
#pragma warning disable CS8602 // 解引用可能出现空引用。
                    Bitmap icon = System.Drawing.Icon.ExtractAssociatedIcon(item).ToBitmap();
#pragma warning restore CS8602 // 解引用可能出现空引用。
                    App.contentStackPanel = ContentStackPanel;
                    AddFile(item, System.Windows.Interop.Imaging
                        .CreateBitmapSourceFromHBitmap(icon.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()));
                }
                catch
                {
                    continue;
                }
            }
            this.SetWindowPos(appWindowClass.screenPart);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadFilesFromString(filePath);
        }
    }
}
