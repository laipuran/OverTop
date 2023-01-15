using Newtonsoft.Json;
using OverTop.Floatings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using static OverTop.AppWindowOps;
using static OverTop.CommonWindowOps;
using Point = System.Windows.Point;

namespace OverTop
{
    public static class ExtentedWindowOps
    {
        public static WeatherWindowProperty Save(this WeatherWindow window)
        {
            WeatherWindowProperty windowClass = new();
            windowClass.left = window.Left;
            windowClass.top = window.Top;
            if (window.Visibility == Visibility.Visible)
            {
                windowClass.isVisible = true;
            }
            else windowClass.isVisible = false;

            return windowClass;
        }

        public static RecentWindowProperty Save(this RecentWindow window)
        {
            RecentWindowProperty windowClass = new();
            System.Windows.Media.Color color = ((SolidColorBrush)window.Background).Color;
            windowClass.backgroundColor = System.Drawing.ColorTranslator.ToHtml(System.Drawing.Color.FromArgb(color.R, color.G, color.B));
            windowClass.width = (int)window.Width;
            windowClass.height = (int)window.Height;
            windowClass.alpha = window.Opacity;
            windowClass.left = window.Left;
            windowClass.top = window.Top;
            windowClass.isTop = window.Topmost;
            return windowClass;
        }

        public static void ToBottom(this Window window)
        {
            CommonWindowOps.SetWindowPos(new WindowInteropHelper(window).Handle,
                (IntPtr)WindowZIndex.Bottom, (int)window.Left,
                (int)window.Top, (int)window.Width, (int)window.Height, 0);
        }

        public static void ToTop(this Window window)
        {
            CommonWindowOps.SetWindowPos(new WindowInteropHelper(window).Handle,
                (IntPtr)WindowZIndex.TopMost, (int)window.Left,
                (int)window.Top, (int)window.Width, (int)window.Height, 0);
        }

        public static HangerWindowProperty? Save(this HangerWindow window)
        {
            HangerWindowProperty windowClass = new();
            System.Windows.Media.Color color = ((SolidColorBrush)window.Background).Color;
            windowClass.backgroundColor = System.Drawing.ColorTranslator.ToHtml(System.Drawing.Color.FromArgb(color.R, color.G, color.B));
            windowClass.width = (int)window.Width;
            windowClass.height = (int)window.Height;
            windowClass.alpha = window.Opacity;
            windowClass.left = window.Left;
            windowClass.top = window.Top;
            windowClass.guid = window.Title;
            windowClass.isTop = window.Topmost;

            if (window.ContentStackPanel.Children.Count == 0)
            {
                return null;
            }

            foreach (StackPanel item in window.ContentStackPanel.Children)
            {
                if (item.Children[0] is TextBlock)
                {
                    windowClass.contents.Add(new(HangerWindowProperty.ContentType.Text, ((TextBlock)item.Children[0]).Text));
                }
                else if (item.Children[0] is System.Windows.Controls.Image)
                {
                    windowClass.contents.Add(new(HangerWindowProperty.ContentType.Image,
                        ((System.Windows.Controls.Image)item.Children[0]).Source.ToString()));
                }
            }
            return windowClass;
        }

        public static void Save(this AppWindow window)
        {
            AppWindowOps appWindow = new();
            appWindow.screenPart = window.GetMiddlePoint().GetPart();
            foreach (StackPanel item in window.ContentStackPanel.Children)
            {
#pragma warning disable CS8604 // 引用类型参数可能为 null。
                appWindow.filePath.Add(item.ToolTip.ToString());
#pragma warning restore CS8604 // 引用类型参数可能为 null。
            }
            string json = JsonConvert.SerializeObject(appWindow);
            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\OverTop\\AppWindow.json";
#pragma warning disable CS8602 // 解引用可能出现空引用。
            Directory.CreateDirectory(Directory.GetParent(filePath).FullName);
#pragma warning restore CS8602 // 解引用可能出现空引用。
            File.WriteAllText(filePath, json);
        }

        public static Point GetMiddlePoint(this Window window)
        {
            Point point = new();
            point.X = window.Left + window.Width / 2;
            point.Y = window.Top + window.Height / 2;

            return point;
        }

        public static AppWindowOps.ScreenPart GetPart(this Point point)
        {
            // Can automatically change when dpi is changed
            Point screenSize = new();
            screenSize.X = SystemParameters.PrimaryScreenWidth;
            screenSize.Y = SystemParameters.PrimaryScreenHeight;

            if (point.X < screenSize.X / 4)
            {
                return ScreenPart.LeftPart;
            }
            else if (point.X > screenSize.X * 3 / 4)
            {
                return ScreenPart.RightPart;
            }
            else if (point.Y < screenSize.Y / 2)
            {
                return ScreenPart.TopPart;
            }
            else // if (point.Y > screenSize.Y / 2)
            {
                return ScreenPart.BottomPart;
            }
        }

        public static void SetWindowPos(this AppWindow window, ScreenPart part)
        {
            if (part == ScreenPart.TopPart)
            {
                window.Height = 60;
                window.Width = 560;
                window.ContentStackPanel.Orientation = System.Windows.Controls.Orientation.Horizontal;
                window.Top = 0;
                window.Left = (SystemParameters.FullPrimaryScreenWidth - window.Width) / 2;
            }
            else if (part == ScreenPart.BottomPart)
            {
                window.Height = 60;
                window.Width = 560;
                window.ContentStackPanel.Orientation = System.Windows.Controls.Orientation.Horizontal;
                window.Top = SystemParameters.FullPrimaryScreenHeight - window.Height;
                window.Left = (SystemParameters.FullPrimaryScreenWidth - window.Width) / 2;
            }
            else if (part == ScreenPart.LeftPart)
            {
                window.Height = 560;
                window.Width = 60;
                window.ContentStackPanel.Orientation = System.Windows.Controls.Orientation.Vertical;
                window.Top = (SystemParameters.FullPrimaryScreenHeight - window.Height) / 2;
                window.Left = 0;
            }
            else
            {
                window.Height = 560;
                window.Width = 60;
                window.ContentStackPanel.Orientation = System.Windows.Controls.Orientation.Vertical;
                window.Top = (SystemParameters.FullPrimaryScreenHeight - window.Height) / 2;
                window.Left = SystemParameters.FullPrimaryScreenWidth - window.Width;
            }
        }
    }

    public class CommonWindowOps
    {
        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndlnsertAfter, int X, int Y, int cx, int cy, uint Flags);

        public enum WindowType
        {
            Hanger = 0,
            Recent = 1
        }

        public enum WindowZIndex
        {
            Top = 0,
            Bottom = 1,
            TopMost = -1,
            NoTopMost = -2,
        }

        public static void ChangeZIndex(bool isBottom, Window window)
        {
            if (isBottom)
            {
                window.ToBottom();
            }
            else
            {
                window.ToTop();
            }
        }

        public static bool ChangeStatus(bool isBottom, Window window)
        {
            if (isBottom)
            {
                window.ToTop();
                window.Topmost = true;
            }
            else
            {
                window.ToBottom();
                window.Topmost = false;
            }
            return !isBottom;
        }
    }

    public class AppWindowOps
    {
        public ScreenPart screenPart;
        public List<string> filePath = new();

        public enum Quadrant // not used
        {
            Quadrant1,
            Quadrant2,
            Quadrant3,
            Quadrant4
        }

        public enum ScreenPart
        {
            TopPart,
            BottomPart,
            RightPart,
            LeftPart
        }

        public static Quadrant GetQuadrant(Point point) // not used
        {
            Point middleScreen = new();
            middleScreen.X = SystemParameters.PrimaryScreenWidth / 2;
            middleScreen.Y = SystemParameters.PrimaryScreenHeight / 2;
            if (point.X > middleScreen.X && point.Y < middleScreen.Y)
            {
                return Quadrant.Quadrant1;
            }
            else if (point.X < middleScreen.X && point.Y < middleScreen.Y)
            {
                return Quadrant.Quadrant2;
            }
            else if (point.X < middleScreen.X && point.Y > middleScreen.Y)
            {
                return Quadrant.Quadrant3;
            }
            else // if (point.X > middleScreen.X && point.Y > middleScreen.Y)
            {
                return Quadrant.Quadrant4;
            }
        }

        public static void AddFile(string path, ImageSource source)
        {
            if (AppWindow.controls.ContainsKey(path))
            {
                return;
            }

            StackPanel appPanel = new();
            System.Windows.Controls.Image image = new();
            image.Source = source;
            image.Width = 40;
            Thickness margin = new(10, 10, 10, 10);
            appPanel.Children.Add(image);
            appPanel.Margin = margin;
            appPanel.ToolTip = path;
            appPanel.MouseLeftButtonDown += AppWindow.AppPanel_MouseLeftButtonDown;
            appPanel.AllowDrop = true;

            if (AppWindow.controls.Count >= 9)
            {
                return;
            }

            AppWindow.controls.Add(path, appPanel);
            App.contentStackPanel.Children.Add(appPanel);
        }
    }

}
