using Newtonsoft.Json;
using OverTop.Floatings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;

namespace OverTop
{
    public class CommonWindowOps
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
    public class HangerWindowOps
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
            HangerWindowOps windowClass = new();
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
                    windowClass.contents.Add(new(HangerWindowOps.ContentType.Text, ((TextBlock)item.Children[0]).Text));
                }
                else if (item.Children[0] is System.Windows.Controls.Image)
                {
                    windowClass.contents.Add(new(HangerWindowOps.ContentType.Image,
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
            AppWindowOps appWindow = new();
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

    public class Apis
    {
        private static string key = "ad768f25db9eb67e3883c2a16f59295b";
        private static string ipSrc = "https://ip.useragentinfo.com/myip";
        private static string amapBase = "https://restapi.amap.com/v3/";

        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class IpInformation
        {
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
            public string status { get; set; }
            public string info { get; set; }
            public string infocode { get; set; }
            public string province { get; set; }
            public string city { get; set; }
            public string adcode { get; set; }
            public string rectangle { get; set; }
        }

        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class Life
        {
            public string province { get; set; }
            public string city { get; set; }
            public string adcode { get; set; }
            public string weather { get; set; }
            public string temperature { get; set; }
            public string winddirection { get; set; }
            public string windpower { get; set; }
            public string humidity { get; set; }
            public string reporttime { get; set; }
        }

        public class WeatherInformation
        {
            public string status { get; set; }
            public string count { get; set; }
            public string info { get; set; }
            public string infocode { get; set; }
            public List<Life> lives { get; set; }
        }

#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

        public static string GetData(string url)
        {
            HttpClient client = new HttpClient();
            string result = client.GetStringAsync(url).Result;
            return result;
        }

        public static string GetHostIp()
        {
            string? ip = GetData(ipSrc);
            if (ip is not null)
                return ip.Substring(0, ip.Length - 1);
            return "";
        }

        public static IpInformation GetIpInformation(string ip)
        {
            string json = GetData(amapBase + "ip?key=" + key + "&ip=" + ip);

#pragma warning disable CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
            IpInformation iP = JsonConvert.DeserializeObject<IpInformation>(json);
#pragma warning restore CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。

#pragma warning disable CS8603 // 可能返回 null 引用。
            return iP;
#pragma warning restore CS8603 // 可能返回 null 引用。
        }

        public static WeatherInformation GetWeatherInformation(string adcode)
        {
            string json = GetData(amapBase + "weather/weatherInfo?key=" + key + "&city=" + adcode);

#pragma warning disable CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
            WeatherInformation weather = JsonConvert.DeserializeObject<WeatherInformation>(json);
#pragma warning restore CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。

#pragma warning disable CS8603 // 可能返回 null 引用。
            return weather;
#pragma warning restore CS8603 // 可能返回 null 引用。
        }
    }
}
