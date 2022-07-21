using Newtonsoft.Json;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
            if (System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.LeftCtrl))
            {
                SaveWindow();
            }
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        public void SaveWindow()
        {
            WindowClass windowClass = new();
            System.Windows.Media.Color color = ((SolidColorBrush)Background).Color;
            windowClass.backgroundColor = System.Drawing.ColorTranslator.ToHtml(System.Drawing.Color.FromArgb(color.R, color.G, color.B));
            windowClass.width = (int)Width;
            windowClass.height = (int)Height;
            windowClass.alpha = Opacity;
            windowClass.left = Left;
            windowClass.top = Top;
            int index = 0;
            foreach (var item in ContentStackPanel.Children)
            {
                if (item is TextBlock)
                {
                    windowClass.contents.Add(index, ((TextBlock)item).Text);
                }
                else if (item is System.Windows.Controls.Image)
                {
                    windowClass.contents.Add(index, ((System.Windows.Controls.Image)item).Source.ToString());
                    // TODO: Change sourcep path to base64
                }
                index++;
            }
            string json = JsonConvert.SerializeObject(windowClass);
            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\OverTop\\" + Guid.NewGuid().ToString() + ".json";
            Directory.CreateDirectory(Directory.GetParent(filePath).FullName);
            FileStream fileStream = File.OpenWrite(filePath);
            fileStream.Write(Encoding.UTF8.GetBytes(json));
            fileStream.Close();

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
