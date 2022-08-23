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
        
        // The Variables needed in the whole application
        public static CommonWindowOps.WindowType currentWindowType = new();

        public static Window currentWindow = new();
        public static StackPanel contentStackPanel = new();

        public static MainWindow? mainWindow;
        public static AppWindow appWindow = new();
        public static WeatherWindow weatherWindow = new();

        public static Property tempProperty = new();
        public static Settings settings = new();

        public static string ip = "";

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

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            SaveSettings(App.settings);
        }
    }
}
