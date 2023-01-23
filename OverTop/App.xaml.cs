using OverTop.Floatings;
using OverTop.Pages;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
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
        public static AppWindow AppWindow = new();
        public static WeatherWindow WeatherWindow = new(WeatherWindowProperty.GetDefaultProperty());

        public static Settings AppSettings = new();

        public static string? CurrentIP = "";

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
            }

            if (AppSettings.RecentWindow is not null)
            {
                FloatingPanelPage.recents++;
                RecentWindow newRecent = (RecentWindow)AppSettings.RecentWindow.GetWindow();
                newRecent.Show();
            }

            if (AppSettings.HangerWindows is not null)
            {
                foreach (HangerWindowProperty windowClass in AppSettings.HangerWindows)
                {
                    FloatingPanelPage.hangers++;
                    HangerWindow newHanger = (HangerWindow)windowClass.GetWindow();
                    newHanger.Show();
                }
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            App.AppSettings.Save();
        }
    }
}
