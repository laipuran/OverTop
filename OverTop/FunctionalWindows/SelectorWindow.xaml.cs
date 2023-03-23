using Microsoft.Win32;
using OverTop.ContentWindows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace OverTop.FunctionalWindows
{
    /// <summary>
    /// ChooserWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SelectorWindow : Window
    {
        List<String> filePaths = new();
        List<String> folders = new()
        {
            Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu),
            Environment.GetFolderPath(Environment.SpecialFolder.StartMenu),
            Environment.GetFolderPath(Environment.SpecialFolder.CommonPrograms),
            Environment.GetFolderPath(Environment.SpecialFolder.Programs)
        };
        DockWindow Dock;

        public SelectorWindow(DockWindow window)
        {
            InitializeComponent();
            Task.Run(() => Dispatcher.BeginInvoke(() => { GetShortCuts(); }));

            Dock = window;
        }

        #region Show Icons
        private void GetFiles(string path)
        {
            try
            {
                foreach (string file in Directory.GetFiles(path))
                {
                    if (!file.EndsWith(".lnk"))
                    {
                        continue;
                    }
                    filePaths.Add(file);
                }
                string[] directories = Directory.GetDirectories(path);
                if (directories.Length >= 0)
                {
                    foreach (string directory in directories)
                    {
                        GetFiles(directory);
                    }
                }
            }
            catch { }
        }

        private async void GetShortCuts()
        {
            SortedDictionary<string, string> fileInfo = new();
            foreach (string folder in folders)
            {
                GetFiles(folder);
            }

            foreach (string file in filePaths)
            {
                IWshRuntimeLibrary.WshShell shell = new();
                IWshRuntimeLibrary.IWshShortcut wshShortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(file);
                if (!wshShortcut.TargetPath.EndsWith(".exe") || !File.Exists(wshShortcut.TargetPath))
                {
                    continue;
                }
                string fileName = Path.GetFileNameWithoutExtension(wshShortcut.TargetPath);
                if (fileInfo.ContainsKey(fileName) || fileName.StartsWith("unins") ||
                    fileName.StartsWith("Unins") || fileName.EndsWith("ninstall") ||
                    fileName.EndsWith("ninst") || fileName.EndsWith("ninstaller"))
                {
                    continue;
                }
                fileInfo.Add(Path.GetFileNameWithoutExtension(wshShortcut.TargetPath), wshShortcut.TargetPath);
            }

            foreach (KeyValuePair<string, string> keyPair in fileInfo)
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
                if (!File.Exists(keyPair.Value))
                {
                    continue;
                }
                try
                {
                    System.Drawing.Icon? icon = System.Drawing.Icon.ExtractAssociatedIcon(keyPair.Value);
                    if (icon is null)
                        continue;
                    Bitmap bitmap = icon.ToBitmap();
                    BitmapSource bitmapSource = System.Windows.Interop.Imaging
                        .CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    image.Source = bitmapSource;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, keyPair.Key);
                }

                TextBlock textBlock = new()
                {
                    Text = keyPair.Key,
                    Style = (Style)FindResource("ContentTextBlockStyle"),
                    Margin = margin
                };
                itemStackPanel.Children.Add(image);
                itemStackPanel.Children.Add(textBlock);
                itemStackPanel.ToolTip = keyPair.Value;
                itemStackPanel.MouseLeftButtonDown += ItemStackPanel_MouseLeftButtonDown;

                await Task.Run(() => Dispatcher.BeginInvoke(new Action(() => { ContentStackPanel.Children.Add(itemStackPanel); })));
            }

        }

        #endregion

        private void ItemStackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string? path = ((StackPanel)sender).ToolTip.ToString();
            if (path is null)
                return;
            if (!File.Exists(path))
            {
                ContentStackPanel.Children.Remove((StackPanel)sender);
                return;
            }
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                DirectoryInfo? info = Directory.GetParent(path);
                if (info is not null)
                    Process.Start("explorer.exe", info.ToString());
            }
            else
                Dock.Property.AddApplication(path);
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
                    Dock.Property.AddApplication(fileName);
                }
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.R))
            {
                ContentStackPanel.Children.Clear();
                filePaths.Clear();
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
