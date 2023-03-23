﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;

namespace OverTop
{
    public class WebApi
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

        public static string? GetData(string url)
        {
            string? result = null;
            try
            {
                HttpClient client = new HttpClient();
                result = client.GetStringAsync(url).Result;
            }
            catch { }
            return result;
        }

        public static string? GetHostIp()
        {
            string? ip = GetData(ipSrc);
            if (ip is not null)
                return ip.Substring(0, ip.Length);
            return null;
        }

        public static IpInformation? GetIpInformation(string ip)
        {
            string? json = GetData(amapBase + "ip?key=" + key + "&ip=" + ip);
            if (json is null) return null;
            IpInformation? i2 = new();
            try
            {
                i2 = JsonConvert.DeserializeObject<IpInformation>(json);
            }
            catch { }

            return i2 is null ? null : i2;
        }

        public static WeatherInformation? GetWeatherInformation(string adcode)
        {
            string? json = GetData(amapBase + "weather/weatherInfo?key=" + key + "&city=" + adcode);
            if (json is null) return null;

            WeatherInformation? weather = JsonConvert.DeserializeObject<WeatherInformation>(json);

            return weather is null ? null : weather;
        }
    }

    public class WindowEnumerator
    {
        /// <summary>
        /// 查找当前用户空间下所有符合条件的窗口。如果不指定条件，将仅查找可见窗口。
        /// </summary>
        /// <param name="match">过滤窗口的条件。如果设置为 null，将仅查找可见窗口。</param>
        /// <returns>找到的所有窗口信息。</returns>
        public static IReadOnlyList<WindowInfo> FindAll(Predicate<WindowInfo>? match = null)
        {
            var windowList = new List<WindowInfo>();
            EnumWindows(OnWindowEnum, 0);
            return windowList.FindAll(match ?? DefaultPredicate);

            bool OnWindowEnum(IntPtr hWnd, int lparam)
            {
                // 仅查找顶层窗口。
                if (GetParent(hWnd) == IntPtr.Zero)
                {
                    // 获取窗口类名。
                    var lpString = new StringBuilder(512);
                    GetClassName(hWnd, lpString, lpString.Capacity);
                    var className = lpString.ToString();

                    // 获取窗口标题。
                    var lptrString = new StringBuilder(512);
                    GetWindowText(hWnd, lptrString, lptrString.Capacity);
                    var title = lptrString.ToString().Trim();

                    // 获取窗口可见性。
                    var isVisible = IsWindowVisible(hWnd);

                    // 获取窗口位置和尺寸。
                    LPRECT rect = default;
                    GetWindowRect(hWnd, ref rect);
                    var bounds = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);

                    // 添加到已找到的窗口列表。
                    windowList.Add(new WindowInfo(hWnd, className, title, isVisible, bounds));
                }

                return true;
            }
        }

        /// <summary>
        /// 默认的查找窗口的过滤条件。可见 + 非最小化 + 包含窗口标题。
        /// </summary>
        private static readonly Predicate<WindowInfo> DefaultPredicate = x => x.IsVisible && !x.IsMinimized && x.Title.Length > 0;

        private delegate bool WndEnumProc(IntPtr hWnd, int lParam);

        [DllImport("user32")]
        private static extern bool EnumWindows(WndEnumProc lpEnumFunc, int lParam);

        [DllImport("user32")]
        private static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lptrString, int nMaxCount);

        [DllImport("user32")]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32")]
        private static extern bool GetWindowRect(IntPtr hWnd, ref LPRECT rect);

        [StructLayout(LayoutKind.Sequential)]
        private readonly struct LPRECT
        {
            public readonly int Left;
            public readonly int Top;
            public readonly int Right;
            public readonly int Bottom;
        }
    }

    /// <summary>
    /// 获取 Win32 窗口的一些基本信息。
    /// </summary>
    public readonly struct WindowInfo
    {
        public WindowInfo(IntPtr hWnd, string className, string title, bool isVisible, Rectangle bounds) : this()
        {
            Hwnd = hWnd;
            ClassName = className;
            Title = title;
            IsVisible = isVisible;
            Bounds = bounds;
        }

        /// <summary>
        /// 获取窗口句柄。
        /// </summary>
        public IntPtr Hwnd { get; }

        /// <summary>
        /// 获取窗口类名。
        /// </summary>
        public string ClassName { get; }

        /// <summary>
        /// 获取窗口标题。
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// 获取当前窗口是否可见。
        /// </summary>
        public bool IsVisible { get; }

        /// <summary>
        /// 获取窗口当前的位置和尺寸。
        /// </summary>
        public Rectangle Bounds { get; }

        /// <summary>
        /// 获取窗口当前是否是最小化的。
        /// </summary>
        public bool IsMinimized => Bounds.Left == -32000 && Bounds.Top == -32000;
    }
}
