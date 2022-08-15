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

        string startMenu = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu);
        public ChooserWindow()
        {
            InitializeComponent();

            GetShortCuts();
        }

        private async void GetShortCuts()
        {
            List<string> filePath = new();
            Dictionary<string, Bitmap> fileInfo = new();
            try
            {

                foreach (string file in Directory.GetFiles(startMenu))
                {
                    filePath.Add(file);
                }

                foreach (string folder in Directory.GetDirectories(startMenu))
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
                if (!file.EndsWith(".lnk"))
                {
                    continue;
                }
                IWshRuntimeLibrary.WshShell shell = new();
                IWshRuntimeLibrary.IWshShortcut wshShortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(file);

#pragma warning disable CS8602 // 解引用可能出现空引用。
                Bitmap icon = System.Drawing.Icon.ExtractAssociatedIcon(wshShortcut.TargetPath).ToBitmap();
#pragma warning restore CS8602 // 解引用可能出现空引用。
                fileInfo.Add(wshShortcut.TargetPath, icon);

            }

            foreach (KeyValuePair<string, Bitmap> keyPair in fileInfo)
            {
                Thickness margin = new(10, 0, 0, 0);
                Thickness panelMargin = new(5, 5, 5, 5);
                StackPanel newStackPanel = new()
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
                    Text = System.IO.Path.GetFileName(keyPair.Key),
                    ToolTip = keyPair.Key,
                    Style = (Style)FindResource("ContentTextBlockStyle"),
                    Margin = margin
                };
                newStackPanel.Children.Add(image);
                newStackPanel.Children.Add(textBlock);

                newStackPanel.MouseLeftButtonDown += NewStackPanel_MouseLeftButtonDown;

                await Task.Run(() => Dispatcher.BeginInvoke(new Action(() => { ContentStackPanel.Children.Add(newStackPanel); })));
            }

        }

        private void NewStackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string path = (string)((TextBlock)((StackPanel)sender).Children[1]).ToolTip;
            if (AppWindow.controls.ContainsKey(path))
            {
                return;
            }
            StackPanel appPanel = new();
            System.Windows.Controls.Image image = new();
            image.Source = ((System.Windows.Controls.Image)((StackPanel)sender).Children[0]).Source;
            image.Width = 40;
            Thickness margin = new(10, 10, 10, 10);
            appPanel.Children.Add(image);
            appPanel.Margin = margin;
            appPanel.ToolTip = path;
            appPanel.MouseLeftButtonDown += AppPanel_MouseLeftButtonDown;
            appPanel.AllowDrop = true;
            appPanel.Drop += AppPanel_Drop;
            AppWindow.controls.Add(path, appPanel);
            App.contentStackPanel.Children.Add(appPanel);
        }

        private void AppPanel_Drop(object sender, DragEventArgs e)
        {
            Process[] processes = Process.GetProcessesByName((Path.GetFileNameWithoutExtension(((System.Windows.Controls.Image)((StackPanel)sender).Children[0]).ToolTip.ToString())));
            foreach (Process item in processes)
            {
                IntPtr hWnd = item.Handle;
                ShowWindowAsync(hWnd, 5);
            }
        }

        private void AppPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
#pragma warning disable CS8604 // 引用类型参数可能为 null。
            Process.Start("explorer.exe", ((StackPanel)sender).ToolTip.ToString());
#pragma warning restore CS8604 // 引用类型参数可能为 null。
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
                    StackPanel appPanel = new();
                    System.Windows.Controls.Image image = new();
#pragma warning disable CS8602 // 解引用可能出现空引用。
                    Bitmap icon = System.Drawing.Icon.ExtractAssociatedIcon(fileName).ToBitmap();
#pragma warning restore CS8602 // 解引用可能出现空引用。
                    image.Source = System.Windows.Interop.Imaging
                        .CreateBitmapSourceFromHBitmap(icon.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    image.Width = 40;
                    Thickness margin = new(10, 10, 10, 10);
                    appPanel.Children.Add(image);
                    appPanel.Margin = margin;
                    appPanel.ToolTip = fileName;
                    appPanel.MouseLeftButtonDown += AppPanel_MouseLeftButtonDown;
                    App.contentStackPanel.Children.Add(appPanel);
                }
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.R))
            {
                ContentStackPanel.Children.Clear();
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
