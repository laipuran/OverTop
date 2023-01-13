using Newtonsoft.Json;
using OverTop.Floatings;
using OverTop.Pages;
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
        public static WeatherWindow weatherWindow = new(WeatherWindowProperty.GetDefaultProperty());

        public static Property tempProperty = new();
        public static Settings settings = new();

        public static string? ip = "";

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            IntPtr hWnd = FindWindow(null, "Over Top");                     //Avoiding opening this app twice
            if (hWnd != IntPtr.Zero)
            {
                MessageBox.Show("Over Top 存在运行中的实例！", "Over Top");
                Environment.Exit(-1);
            }

            ip = API.GetHostIp();
            if (ip is not null)
                settings = GetSettingsFromFile(ip);
            else
                settings = GetDefaultSettings(ip);

            if (settings.WeatherWindow is not null)
            {
                weatherWindow = (WeatherWindow)settings.WeatherWindow.GetWindow();
                if (settings.WeatherWindow.isVisible == true)
                {
                    weatherWindow.Show();
                }
            }

            if (settings.RecentWindow is not null)
            {
                FloatingPanelPage.recents++;
                RecentWindow newRecent = (RecentWindow)settings.RecentWindow.GetWindow();
                newRecent.Show();
            }

            if (settings.HangerWindows is not null)
            {
                foreach (HangerWindowProperty windowClass in settings.HangerWindows)
                {
                    FloatingPanelPage.hangers++;
                    HangerWindow newHanger = (HangerWindow)windowClass.GetWindow();
                    newHanger.Show();
                }
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            App.settings.Save();
        }
    }
}
