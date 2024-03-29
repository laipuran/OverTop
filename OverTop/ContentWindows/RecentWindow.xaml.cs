﻿using OverTop.FunctionalWindows;
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
using static OverTop.CommonWindowOps;
using static PuranLai.Tools.ExtendedWindowOps;

namespace OverTop.ContentWindows
{
    /// <summary>
    /// RecentWindow.xaml 的交互逻辑
    /// </summary>
    public partial class RecentWindow : Window
    {
        public bool isMouseIn = false;
        public RecentWindowProperty Property;
        public StackPanel ContentPanel;
        string RecentPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Microsoft\Windows\Recent\";
        public RecentWindow(RecentWindowProperty property)
        {
            InitializeComponent();
            Property = property;
            App.Floatings.Add(this);
            ContentPanel = ContentStackPanel;
            if (!Property.isTop)
            {
                ChangeStatus(false, this);
            }
            Task.Run(() => Dispatcher.BeginInvoke(() => { LoadRecentFiles(); }));

            this.Reload();
        }

        // Get system recent files
        private async void LoadRecentFiles()
        {
            try
            {
                Dictionary<string, Bitmap> fileInfo = new();
                string[] files = Directory.GetFiles(RecentPath);
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
                    StackPanel LinkStackPanel = new()
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
                    LinkStackPanel.Children.Add(image);
                    LinkStackPanel.Children.Add(textBlock);

                    LinkStackPanel.ToolTip = file.Key;
                    LinkStackPanel.MouseLeftButtonDown += LinkStackPanel_MouseLeftButtonDown;

                    await Task.Run(() => Dispatcher.BeginInvoke(new Action(() => { ContentStackPanel.Children.Add(LinkStackPanel); })));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void LinkStackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                string filePath = RecentPath + ((TextBlock)((StackPanel)sender).Children[1]).Text + ".lnk";
                IWshRuntimeLibrary.WshShell shell = new();
                IWshRuntimeLibrary.IWshShortcut wshShortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(filePath);
                Process.Start("explorer.exe", wshShortcut.TargetPath);
            }
            else if (Keyboard.IsKeyDown(Key.Left))
                Left -= 5;
            else if (Keyboard.IsKeyDown(Key.Right))
                Left += 5;
            else if (Keyboard.IsKeyDown(Key.Up))
                Top -= 5;
            else if (Keyboard.IsKeyDown(Key.Down))
                Top += 5;

        }

        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            PropertyWindow propertyWindow = new PropertyWindow(this)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            propertyWindow.ShowDialog();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.R))
            {
                ContentStackPanel.Children.Clear();
                Task.Run(() => Dispatcher.BeginInvoke(new Action(LoadRecentFiles)));
            }
            else if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private unsafe void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            CommonWindowOps.ChangeZIndex(Property.isTop, this);
            Scroller.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            fixed (bool* MouseIn = &isMouseIn)
            {
                this.ChangeOpacity(OpacityOptions._75, MouseIn);
            }
        }

        private unsafe void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            CommonWindowOps.ChangeZIndex(Property.isTop, this);
            Scroller.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            fixed (bool* MouseIn = &isMouseIn)
            {
                this.ChangeOpacity(OpacityOptions._25, MouseIn);
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Tab)
            {
                Property.isTop = CommonWindowOps.ChangeStatus(Property.isTop, this);
                return;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            App.Floatings.Remove(this);
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            this.Property.Top = Top;
            this.Property.Left = Left;
        }
    }
}