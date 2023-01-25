using Newtonsoft.Json;
using OverTop.Floatings;
using OverTop.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using static OverTop.AppWindowOps;
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

        public static new MainWindow? MainWindow;
        public static AppWindow AppWindow = (AppWindow)new AppWindowProperty().GetWindow();
        public static WeatherWindow WeatherWindow = new(WeatherWindowProperty.GetDefaultProperty());
        public static List<Window> FloatingWindows = new();
        public static Settings AppSettings = new();
        public static int Recents = 0, Hangers = 0;
        public static string? CurrentIP = "";
        #endregion

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            IntPtr hWnd = FindWindow(null, "Over Top");                     //Avoiding opening this app twice
            if (hWnd != IntPtr.Zero)
            {
                MessageBox.Show("Over Top 存在运行中的实例！", "Over Top");
                Environment.Exit(-1);
            }

            CurrentIP = API.GetHostIp();
            if (CurrentIP is not null)
                AppSettings = GetSettingsFromFile(CurrentIP);
            else
                AppSettings = GetDefaultSettings(CurrentIP);

            if (AppSettings.WeatherWindow is not null)
            {
                WeatherWindow = (WeatherWindow)AppSettings.WeatherWindow.GetWindow();
                if (AppSettings.WeatherWindow.isVisible == true)
                {
                    WeatherWindow.Show();
                }
                FloatingWindows.Add(WeatherWindow);
            }

            if (AppSettings.RecentWindow is not null)
            {
                RecentWindow newRecent = new(AppSettings.RecentWindow);
                newRecent.Show();
            }

            if (AppSettings.HangerWindows is not null)
            {
                foreach (HangerWindowProperty windowClass in AppSettings.HangerWindows)
                {
                    HangerWindow newHanger = new(windowClass);
                    newHanger.Show();
                }
            }

            Pages.StaticPropertyPage.ColorChanged();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            App.AppWindow.Save();

            int RecentWindows = 0, HangerWindows = 0;
            App.AppSettings.HangerWindows = new();
            foreach (System.Windows.Window window in App.FloatingWindows)
            {
                if (window is RecentWindow)
                {
                    RecentWindows++;
                    App.AppSettings.RecentWindow = ((RecentWindow)window).Property;
                }
                else if (window is HangerWindow)
                {
                    HangerWindowProperty? property = ((HangerWindow)window).Property;
                    App.AppSettings.HangerWindows.Add(property);
                    HangerWindows++;
                }
            }
            if (RecentWindows == 0) App.AppSettings.RecentWindow = null;
            if (HangerWindows == 0) App.AppSettings.HangerWindows = null;
            App.AppSettings.WeatherWindow = App.WeatherWindow.Property;

            App.AppSettings.Save();
        }
    }
}
