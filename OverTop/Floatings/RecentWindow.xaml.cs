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
using System.Windows.Media;
using System.Windows.Media.Imaging;
namespace OverTop.Floatings
{
    /// <summary>
    /// RecentWindow.xaml 的交互逻辑
    /// </summary>
    public partial class RecentWindow : Window
    {
        public const int HWND_TOP = 0;
        public const int HWND_BOTTOM = 1;
        public const int HWND_TOPMOST = -1;
        public const int HWND_NOTOPMOST = -2;
        IntPtr hWnd = new();
        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndlnsertAfter, int X, int Y, int cx, int cy, uint Flags);
        private bool isChild = false;
        Dictionary<string, Bitmap> fileInfo = new();
        string Recent = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Microsoft\Windows\Recent\";
        public RecentWindow()
        {
            InitializeComponent();
            Task.Run(() => Dispatcher.BeginInvoke(new Action(ProcessRecentFiles)));
        }
        // Get system recent files
        private void ProcessRecentFiles()
        {
            try
            {
                string[] files = Directory.GetFiles(Recent);
                foreach (string file in files)
                {
                    string name = System.IO.Path.GetFileName(file);
                    string filePath = Recent + name;
                    if (!filePath.EndsWith(".lnk"))
                    {
                        continue;
                    }
                    IWshRuntimeLibrary.WshShell shell = new();
                    IWshRuntimeLibrary.IWshShortcut wshShortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(filePath);
#pragma warning disable CS8602 // 解引用可能出现空引用。
                    Bitmap icon = System.Drawing.Icon.ExtractAssociatedIcon(wshShortcut.TargetPath).ToBitmap();
#pragma warning restore CS8602 // 解引用可能出现空引用。
                    if (!File.Exists(wshShortcut.TargetPath))
                    {
                        continue;
                    }
                    try
                    {
                        fileInfo.Add(Path.GetFileName(wshShortcut.TargetPath), icon);
                    }
                    catch { }
                }
                foreach (KeyValuePair<string, Bitmap> file in fileInfo)
                {
                    Thickness margin = new(10, 0, 0, 0);
                    Thickness margin1 = new(5, 5, 5, 5);
                    StackPanel newStackPanel = new()
                    {
                        Height = 30,
                        Margin = margin1,
                        Orientation = Orientation.Horizontal
                    };
                    System.Windows.Controls.Image image = new();
                    BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(file.Value.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    image.Source = bitmapSource;
                    TextBlock textBlock = new()
                    {
                        Text = file.Key,
                        Style = (Style)FindResource("ContentTextBlockStyle"),
                        Margin = margin
                    };
                    newStackPanel.Children.Add(image);
                    newStackPanel.Children.Add(textBlock);

                    IWshRuntimeLibrary.WshShell shell = new();
                    string filePath = Recent + textBlock.Text + ".lnk";
                    IWshRuntimeLibrary.IWshShortcut wshShortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(filePath);
                    newStackPanel.ToolTip = wshShortcut.TargetPath;
                    newStackPanel.MouseLeftButtonDown += NewStackPanel_MouseLeftButtonDown;

                    ContentStackPanel.Children.Add(newStackPanel);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void NewStackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                string filePath = Recent + ((TextBlock)((StackPanel)sender).Children[1]).Text + ".lnk";
                IWshRuntimeLibrary.WshShell shell = new();
                IWshRuntimeLibrary.IWshShortcut wshShortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(filePath);
                Process.Start("explorer.exe", wshShortcut.TargetPath);
            }
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
            if (isChild)
            {
                ToBottom();
            }
            Scroller.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            if (isChild)
            {
                ToBottom();
            }
            Scroller.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
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
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
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

        private void ContentStackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}