﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace OverTop.Floatings
{
    /// <summary>
    /// RecentWindow.xaml 的交互逻辑
    /// </summary>
    public partial class RecentWindow : Window
    {
        Dictionary<string, Bitmap> fileInfo = new();
        string Recent = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Microsoft\Windows\Recent\";
        public RecentWindow()
        {
            InitializeComponent();
            Width = RecentSettings.Default.width;
            Height = RecentSettings.Default.height;
            System.Windows.Media.Color color = System.Windows.Media.Color.FromRgb(RecentSettings.Default.color.R, RecentSettings.Default.color.G, RecentSettings.Default.color.B);
            Background = new SolidColorBrush(color);
            Opacity = RecentSettings.Default.alpha == 0.0 ? 0.8 : RecentSettings.Default.alpha;

            ProcessRecentFiles();
        }
        // Get system recent files
        private void ProcessRecentFiles()
        {
            string[] files = Directory.GetFiles(Recent);

            foreach (string file in files)
            {
                string name = System.IO.Path.GetFileName(file);
#pragma warning disable CS8602 // 解引用可能出现空引用。
                Bitmap icon = System.Drawing.Icon.ExtractAssociatedIcon(file).ToBitmap();
#pragma warning restore CS8602 // 解引用可能出现空引用。
                string filePath = Recent + name;
                IWshRuntimeLibrary.WshShell shell = new();
                IWshRuntimeLibrary.IWshShortcut wshShortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(filePath);
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
            Scroller.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            Scroller.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        private void ContentStackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}