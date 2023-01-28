﻿using Newtonsoft.Json;
using PuranLai.Tools;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static OverTop.CommonWindowOps;

namespace OverTop.Floatings
{
    /// <summary>
    /// HangerWindow.xaml 的交互逻辑
    /// </summary>
    public partial class HangerWindow : Window
    {
        public bool isMouseIn = false;
        public HangerWindowProperty Property;
        public StackPanel ContentPanel;

        public HangerWindow(HangerWindowProperty property)
        {
            InitializeComponent();
            Property = property;
            App.FloatingWindows.Add(this);
            this.ToolTip = "Hanger Window - " + this.Property.guid;

            if (!Property.isTop)
            {
                Property.isTop = ChangeStatus(Property.isTop, this);
            }
            ContentPanel = ContentStackPanel;

            ReloadWindow(this, this.Property);
        }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.LeftCtrl))
            {
                string json = JsonConvert.SerializeObject(Property);
                string filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\OverTop\\HangerWindows\\" + Title + ".json";
                DirectoryInfo? info = Directory.GetParent(filePath);
                if (info is null)
                    return;
                Directory.CreateDirectory(info.FullName);
                File.WriteAllText(filePath, json);
            }
            else if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Tab)
            {
                Property.isTop = CommonWindowOps.ChangeStatus(Property.isTop, this);
                return;
            }
            try
            {
                switch (e.Key)
                {
                    case Key.Left: Left -= 5; break;
                    case Key.Right: Left += 5; break;
                    case Key.Up: Top -= 5; break;
                    case Key.Down: Top += 5; break;
                    case Key.Escape: Close(); break;
                    default: break;
                        //TODO: Fix needed
                }
            }
            catch { }
        }

        private unsafe void Window_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            CommonWindowOps.ChangeZIndex(Property.isTop, this);
            Scroller.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            fixed (bool* MouseIn = &isMouseIn)
            {
                this.ChangeOpacity(ExtendedWindowOps.OpacityOptions._75, MouseIn);
            }
        }

        private unsafe void Window_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            CommonWindowOps.ChangeZIndex(Property.isTop, this);
            Scroller.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            fixed (bool* MouseIn = &isMouseIn)
            {
                if (this.Property.contents.Count == 0)
                    this.ChangeOpacity(5, MouseIn);
                else
                    this.ChangeOpacity(ExtendedWindowOps.OpacityOptions._25, MouseIn);
            }
            Property.top = Top;
            Property.left = Left;
        }

        private void Window_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Window propertyWindow = new PropertyWindow(this)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            propertyWindow.Show();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            App.FloatingWindows.Remove(this);
        }
    }
}
