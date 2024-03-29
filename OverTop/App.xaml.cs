﻿using Newtonsoft.Json;
using OverTop.ContentWindows;
using OverTop.FunctionalWindows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
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

        #region The variables needed in the whole application
        public static new MainWindow MainWindow = new();
        public static DockWindow DockPanel = new();
        public static WeatherWindow WeatherWindow = new(WeatherWindowProperty.GetDefaultProperty());
        public static List<Window> Floatings = new();
        public static Settings AppSettings = new();
        public static string? CurrentIP = "";
        #endregion

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            SplashWindow splashScreen = new();
            splashScreen.Show();

            IntPtr hWnd = FindWindow(null, "Over Top");                     //Avoiding opening this app twice
            if (hWnd != IntPtr.Zero)
            {
                MessageBox.Show("Over Top 存在运行中的实例！", "Over Top");
                Environment.Exit(-1);
            }

            CurrentIP = WebApi.GetHostIp();
            if (CurrentIP is not null)
                AppSettings = GetSettings(CurrentIP);
            else
                AppSettings = GetDefaultSettings(CurrentIP);

            if (AppSettings.WeatherWindow is not null)
            {
                WeatherWindow = (WeatherWindow)AppSettings.WeatherWindow.GetWindow();
                if (AppSettings.WeatherWindow.Visibility == true)
                {
                    WeatherWindow.Show();
                }
                Floatings.Add(WeatherWindow);
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
                    CustomWindow newHanger = new(windowClass);
                    newHanger.Show();
                }
            }

            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\OverTop\\AppWindow.json";

            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);

                AppWindowProperty? appWindowClass = JsonConvert.DeserializeObject<AppWindowProperty>(json);
                if (appWindowClass is not null)
                    DockPanel = new(appWindowClass);
                else
                    DockPanel = (DockWindow)new AppWindowProperty().GetWindow();
            }
            else
            {
                DockPanel = (DockWindow)new AppWindowProperty().GetWindow();
            }
            DockPanel.Show();

            Pages.PropertyPage.ColorChanged();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            App.DockPanel.Save();

            int RecentWindows = 0, HangerWindows = 0;
            App.AppSettings.HangerWindows = new();
            foreach (System.Windows.Window window in App.Floatings)
            {
                if (window is RecentWindow)
                {
                    RecentWindows++;
                    App.AppSettings.RecentWindow = ((RecentWindow)window).Property;
                }
                else if (window is CustomWindow)
                {
                    HangerWindowProperty? property = ((CustomWindow)window).Property;
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
