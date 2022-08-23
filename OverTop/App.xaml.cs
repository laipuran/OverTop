using Newtonsoft.Json;
using OverTop.Floatings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static OverTop.API;
using static OverTop.CommonWindowOps;
using static OverTop.Settings;

namespace OverTop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        private extern static IntPtr FindWindow(string? lpClassName, string? lpWindowName);
        
        public static CommonWindowOps.WindowType currentWindowType = new();

        public static Window currentWindow = new();
        public static StackPanel contentStackPanel = new();

        public static MainWindow? mainWindow;

        public static Property parameterClass = new(),
            recentProperty = new(),
            hangerProperty = new();

        public static Settings settings = new();

        public static AppWindow appWindow = new Floatings.AppWindow();

        public static string ip = "";
        public static IpInformation i2 = new();

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            IntPtr hWnd = FindWindow(null, "Over Top");                     //Avoiding opening this app twice
            if (hWnd != IntPtr.Zero)
            {
                MessageBox.Show("Over Top 存在运行中的实例！", "Over Top");
                Environment.Exit(-1);
            }

            ip = API.GetHostIp();
            settings = GetSettingsFromFile(ip);
        }

    }
}
