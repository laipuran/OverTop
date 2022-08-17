using Microsoft.Win32;
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

namespace OverTop.Floatings
{
    /// <summary>
    /// ChooserWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ChooserWindow : Window
    {
        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        List<String> folders = new();
        string programs = Environment.GetFolderPath(Environment.SpecialFolder.Programs);
        string startMenu = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);
        public ChooserWindow()
        {
            InitializeComponent();
            GetShortCuts();
            folders.Add(programs);
            folders.Add(startMenu);
        }

        private async void GetShortCuts()
        {
            List<string> filePath = new();
            Dictionary<string, Bitmap> fileInfo = new();
            try
            {
                foreach (string folder in folders)
                {
                    foreach (string file in Directory.GetDirectories(folder))
                    {
                        filePath.Add(file);
                    }
                }

                foreach (string folder in Directory.GetDirectories(programs))
                {
                    foreach (string file in Directory.GetFiles(folder))
                    {
                        filePath.Add(file);
                    }
                }

            }
            catch { }

            foreach (string file in filePath)
            {
                if ((!file.EndsWith(".lnk"))|file.Contains("setup")|file.Contains("ninst")|file.Contains("unins000"))
                {
                    continue;
                }
                IWshRuntimeLibrary.WshShell shell = new();
                IWshRuntimeLibrary.IWshShortcut wshShortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(file);
                if (!wshShortcut.TargetPath.EndsWith(".exe"))
                {
                    continue;
                }
#pragma warning disable CS8602 // 解引用可能出现空引用。
                Bitmap icon = System.Drawing.Icon.ExtractAssociatedIcon(wshShortcut.TargetPath).ToBitmap();
#pragma warning restore CS8602 // 解引用可能出现空引用。
                try
                {
                    fileInfo.Add(wshShortcut.TargetPath, icon);
                }
                catch { }
            }

            foreach (KeyValuePair<string, Bitmap> keyPair in fileInfo)
            {
                Thickness margin = new(10, 0, 0, 0);
                Thickness panelMargin = new(5, 5, 5, 5);
                StackPanel itemStackPanel = new()
                {
                    Height = 30,
                    Margin = panelMargin,
                    Orientation = Orientation.Horizontal
                };
                System.Windows.Controls.Image image = new();
                BitmapSource bitmapSource = System.Windows.Interop.Imaging
                    .CreateBitmapSourceFromHBitmap(keyPair.Value.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                image.Source = bitmapSource;
                
                TextBlock textBlock = new()
                {
                    Text = System.IO.Path.GetFileNameWithoutExtension(keyPair.Key),
                    ToolTip = keyPair.Key,
                    Style = (Style)FindResource("ContentTextBlockStyle"),
                    Margin = margin
                };
                itemStackPanel.Children.Add(image);
                itemStackPanel.Children.Add(textBlock);
                
                itemStackPanel.MouseLeftButtonDown += ItemStackPanel_MouseLeftButtonDown;

                await Task.Run(() => Dispatcher.BeginInvoke(new Action(() => { ContentStackPanel.Children.Add(itemStackPanel); })));
            }

        }

        private void ItemStackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string path = (string)((TextBlock)((StackPanel)sender).Children[1]).ToolTip;
            if (!Directory.Exists(path))
            {
                ((StackPanel)((StackPanel)sender).Parent).Children.Remove(sender as UIElement);
                return;
            }
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
#pragma warning disable CS8602 // 解引用可能出现空引用。
                Process.Start("explorer.exe", Directory.GetParent(path).ToString());
#pragma warning restore CS8602 // 解引用可能出现空引用。
            }
            else
            {
                AppWindowClass.AddFile(path, ((System.Windows.Controls.Image)((StackPanel)sender).Children[0]).Source);
            }
        }


        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Programs);
            OpenFileDialog openFileDialog = new()
            {
                Multiselect = true,
                InitialDirectory = path,
                Filter = "可运行文件|*.exe",
                FilterIndex = 1
            };
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string fileName in openFileDialog.FileNames)
                {
#pragma warning disable CS8602 // 解引用可能出现空引用。
                    Bitmap icon = System.Drawing.Icon.ExtractAssociatedIcon(fileName).ToBitmap();
#pragma warning restore CS8602 // 解引用可能出现空引用。
                    AppWindowClass.AddFile(fileName, System.Windows.Interop.Imaging
                        .CreateBitmapSourceFromHBitmap(icon.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()));
                }
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.R))
            {
                ContentStackPanel.Children.Clear();
                GetShortCuts();
                return;
            }
            DragMove();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }
    }
}
