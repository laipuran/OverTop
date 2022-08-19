using Newtonsoft.Json;
using OverTop.Floatings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
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

        public static void SetWindowPos(AppWindow window, ScreenPart part)
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
            try
            {
                AppWindow.controls.Add(path, appPanel);
            }
            catch
            {
                return;
            }
            App.contentStackPanel.Children.Add(appPanel);
        }
    }

    public class WeatherWindowClass
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class Location
        {
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
            public string country { get; set; }
            public string short_name { get; set; }
            public string province { get; set; }
            public string city { get; set; }
            public string area { get; set; }
            public string isp { get; set; }
            public string net { get; set; }
            public string ip { get; set; }
            public int code { get; set; }
            public string desc { get; set; }
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        }

        public static string GetData(string url)
        {
            HttpClient client = new HttpClient();
#pragma warning disable SYSLIB0014 // 类型或成员已过时
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
#pragma warning restore SYSLIB0014 // 类型或成员已过时
            request.Method = "GET";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader myStreamReader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string data = myStreamReader.ReadToEnd();
            return data;
        }

        public static string GetHostIp()
        {
            string ip = GetData("https://ip.useragentinfo.com/myip");
            return ip.Substring(0, ip.Length - 1);
        }

        public static string GetLocationByIP(string IP)
        {
            string url = "https://ip.useragentinfo.com/json?ip=" + IP;
            string json = GetData(url);
#pragma warning disable CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
            Location location= JsonConvert.DeserializeObject<Location>(json);
#pragma warning restore CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。

#pragma warning disable CS8602 // 解引用可能出现空引用。
            if (location.province.EndsWith("省"))
            {
                return location.province.Substring(0, location.province.Length - 1);
            }
#pragma warning restore CS8602 // 解引用可能出现空引用。

            return location.province;
        }
    }
}
