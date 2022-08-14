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

namespace OverTop
{
    public class WindowClass
    {
        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndlnsertAfter, int X, int Y, int cx, int cy, uint Flags);
        
        public enum WindowType
        {
            Hanger = 0,
            Recent = 1
        }

        public enum WindowZIndex
        {
            HWND_TOP = 0,
            HWND_BOTTOM = 1,
            HWND_TOPMOST = -1,
            HWND_NOTOPMOST = -2,
        }

        private static void ToBottom(Window window)
        {
            SetWindowPos(new WindowInteropHelper(window).Handle,
                (IntPtr)WindowZIndex.HWND_BOTTOM, (int)window.Left,
                (int)window.Top, (int)window.Width, (int)window.Height, 0);
        }

        private static void ToTop(Window window)
        {
            SetWindowPos(new WindowInteropHelper(window).Handle,
                (IntPtr)WindowZIndex.HWND_TOPMOST, (int)window.Left,
                (int)window.Top, (int)window.Width, (int)window.Height, 0);
        }

        public static void ChangeZIndex(bool isBottom, Window window)
        {
            if (isBottom)
            {
                ToBottom(window);
            }
            else
            {
                ToTop(window);
            }
        }
        
        public static bool ChangeStatus(bool isBottom, Window window)
        {
            if (isBottom)
            {
                ToTop(window);
                window.Topmost = true;
            }
            else
            {
                ToBottom(window);
                window.Topmost = true;
            }
            return !isBottom;
        }
    }
    public class HangerWindowClass
    {
        public List<KeyValuePair<ContentType, string>> contents = new();
        public string backgroundColor = "";
        public int width;
        public int height;
        public double alpha;
        public double left;
        public double top;

        public enum ContentType
        {
            Text,
            Image
        }

        public static void SaveWindow(HangerWindow window)
        {
            HangerWindowClass windowClass = new();
            System.Windows.Media.Color color = ((SolidColorBrush)window.Background).Color;
            windowClass.backgroundColor = System.Drawing.ColorTranslator.ToHtml(System.Drawing.Color.FromArgb(color.R, color.G, color.B));
            windowClass.width = (int)window.Width;
            windowClass.height = (int)window.Height;
            windowClass.alpha = window.Opacity;
            windowClass.left = window.Left;
            windowClass.top = window.Top;

            foreach (StackPanel item in window.ContentStackPanel.Children)
            {
                if (item.Children[0] is TextBlock)
                {
                    windowClass.contents.Add(new(HangerWindowClass.ContentType.Text, ((TextBlock)item.Children[0]).Text));
                }
                else if (item.Children[0] is System.Windows.Controls.Image)
                {
                    windowClass.contents.Add(new(HangerWindowClass.ContentType.Image,
                        ((System.Windows.Controls.Image)item.Children[0]).Source.ToString()));
                }
            }
            string json = JsonConvert.SerializeObject(windowClass);
            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\OverTop\\HangerWindows\\" + window.Title + ".json";
#pragma warning disable CS8602 // 解引用可能出现空引用。
            Directory.CreateDirectory(Directory.GetParent(filePath).FullName);
#pragma warning restore CS8602 // 解引用可能出现空引用。
            File.WriteAllText(filePath, json);

        }
    }

    public class AppWindowClass
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

        public static ScreenPart GetPart(Point point)
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

        public static Point GetMiddlePoint(AppWindow window)
        {
            Point point = new();
            point.X = window.Left + window.Width / 2;
            point.Y = window.Top + window.Height / 2;

            return point;
        }

        public static void SaveWindow(AppWindow window)
        {
            AppWindowClass appWindow = new();
            appWindow.screenPart = GetPart(GetMiddlePoint(window));
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
    }
}
