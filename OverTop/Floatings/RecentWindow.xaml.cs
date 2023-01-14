using OverTop.Pages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static OverTop.CommonWindowOps;

namespace OverTop.Floatings
{
    /// <summary>
    /// RecentWindow.xaml 的交互逻辑
    /// </summary>
    public partial class RecentWindow : Window
    {
        private bool isBottom = false;
        string Recent = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Microsoft\Windows\Recent\";
        public RecentWindow(RecentWindowProperty property)
        {
            InitializeComponent();

            FloatingPanelPage.recents++;
            FloatingPanelPage.windows.Add(this);

            this.Top = property.top == 0 ? Top : property.top;
            this.Left = property.left == 0 ? Left : property.left;
            this.Width = property.width;
            this.Height = property.height;
            System.Drawing.Color tempColor = ColorTranslator.FromHtml(property.backgroundColor);
            System.Windows.Media.Color color = System.Windows.Media.Color.FromRgb(tempColor.R, tempColor.G, tempColor.B);
            this.Background = new SolidColorBrush(color);
            this.Opacity = property.alpha == 0.0 ? 0.8 : App.settings.RecentWindowSettings.alpha;
            this.ToolTip = "Recent Window - " + FloatingPanelPage.recents;
            this.Title = "Recent Window";
            if (!property.isTop)
            {
                ChangeStatus(false, this);
            }
            LoadRecentFiles();
        }

        // Get system recent files
        private async void LoadRecentFiles()
        {
            try
            {
                Dictionary<string, Bitmap> fileInfo = new();
                string[] files = Directory.GetFiles(Recent);
                foreach (string filePath in files)
                {
                    if (!filePath.EndsWith(".lnk"))
                    {
                        continue;
                    }
                    IWshRuntimeLibrary.WshShell shell = new();
                    IWshRuntimeLibrary.IWshShortcut wshShortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(filePath);
                    if (!File.Exists(wshShortcut.TargetPath))
                    {
                        continue;
                    }
                    try
                    {
#pragma warning disable CS8602 // 解引用可能出现空引用。
                        Bitmap icon = System.Drawing.Icon.ExtractAssociatedIcon(wshShortcut.TargetPath).ToBitmap();
#pragma warning restore CS8602 // 解引用可能出现空引用。
                        fileInfo.Add(wshShortcut.TargetPath, icon);
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
                    BitmapSource bitmapSource = System.Windows.Interop.Imaging
                        .CreateBitmapSourceFromHBitmap(file.Value.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    image.Source = bitmapSource;
                    TextBlock textBlock = new()
                    {
                        Text = Path.GetFileName(file.Key),
                        Style = (Style)FindResource("ContentTextBlockStyle"),
                        Margin = margin
                    };
                    newStackPanel.Children.Add(image);
                    newStackPanel.Children.Add(textBlock);

                    newStackPanel.ToolTip = file.Key;
                    newStackPanel.MouseLeftButtonDown += NewStackPanel_MouseLeftButtonDown;

                    await Task.Run(() => Dispatcher.BeginInvoke(new Action(() => { ContentStackPanel.Children.Add(newStackPanel); })));
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
            App.currentWindowType = CommonWindowOps.WindowType.Recent;
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
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            CommonWindowOps.ChangeZIndex(isBottom, this);
            Scroller.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            CommonWindowOps.ChangeZIndex(isBottom, this);
            Scroller.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
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
                    //TODO: Fix Needed
                    case Key.Left: Left -= 5; break;
                    case Key.Right: Left += 5; break;
                    case Key.Up: Top -= 5; break;
                    case Key.Down: Top += 5; break;
                    case Key.Escape: Close(); break;
                    default: break;
                }
            }
            catch { }
        }

        private void ContentStackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.R))
            {
                ContentStackPanel.Children.Clear();
                Task.Run(() => Dispatcher.BeginInvoke(new Action(LoadRecentFiles)));
            }
            DragMove();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            FloatingPanelPage.recents--;
            FloatingPanelPage.windows.Remove(this);
        }
    }
}