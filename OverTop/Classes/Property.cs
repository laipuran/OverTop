using OverTop.ContentWindows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using static OverTop.AppWindowOps;

namespace OverTop
{
    interface IProperty
    {
        Window GetWindow();
        void FromProperty(WindowProperty property);
    }

    public class WindowProperty
    {
        public System.Windows.Media.Color backgroundColor;
        public int width;
        public int height;
    }

    public class WeatherWindowProperty : IProperty
    {
        public double left;
        public double top;
        public bool isVisible;

        public Window GetWindow()
        {
            if (this is null)
            {
                return GetDefaultProperty().GetWindow();
            }
            return new WeatherWindow(this);
        }

        public void FromProperty(WindowProperty property)
        {
            // Empty
        }

        public static WeatherWindowProperty GetDefaultProperty()
        {
            WeatherWindowProperty property = new();
            property.left = 30;
            property.top = 30;
            property.isVisible = false;
            return property;
        }
    }

    public class ContentWindowProperty
    {
        public string backgroundColor = "";
        public int width;
        public int height;
        public double left;
        public double top;
        public bool isTop;
    }

    public class RecentWindowProperty : ContentWindowProperty, IProperty
    {
        public Window GetWindow()
        {
            if (this is null)
            {
                return GetDefaultProperty().GetWindow();
            }
            return new RecentWindow(this);
        }

        public void FromProperty(WindowProperty property)
        {
            this.width = property.width;
            this.height = property.height;
            System.Windows.Media.Color tempColor = property.backgroundColor;
            this.backgroundColor = System.Drawing.ColorTranslator.ToHtml(System.Drawing.Color.FromArgb(tempColor.R, tempColor.G, tempColor.B));
        }

        public static RecentWindowProperty GetDefaultProperty()
        {
            RecentWindowProperty property = new();
            property.FromProperty(App.AppSettings.RecentWindowSettings);
            property.isTop = true;
            return property;
        }
    }

    public class HangerWindowProperty : ContentWindowProperty, IProperty
    {
        public List<KeyValuePair<ContentType, string>> contents = new();
        public string guid = "";
        public string link = "";

        public enum ContentType
        {
            Text,
            Image
        }

        public void FromProperty(WindowProperty property)
        {
            this.width = property.width;
            this.height = property.height;
            System.Windows.Media.Color tempColor = property.backgroundColor;
            this.backgroundColor = System.Drawing.ColorTranslator.ToHtml(System.Drawing.Color.FromArgb(tempColor.R, tempColor.G, tempColor.B));
        }

        public static HangerWindowProperty GetDefaultProperty()
        {
            HangerWindowProperty property = new();
            property.FromProperty(App.AppSettings.HangerWindowSettings);
            property.guid = Guid.NewGuid().ToString();
            property.isTop = true;
            return property;
        }

        public Window GetWindow()
        {
            if (this is null)
            {
                return GetDefaultProperty().GetWindow();
            }
            return new CustomWindow(this);
        }
    }

    public class AppWindowProperty : IProperty
    {
        public ScreenPart screenPart;
        public List<string> FilePaths = new();

        public AppWindowProperty()
        {
            screenPart = ScreenPart.TopPart;
        }

        public Window GetWindow()
        {
            return new DockWindow(this);
        }

        public void FromProperty(WindowProperty property)
        {
            // Empty
        }

        public void AddApplication(string path)
        {
            if (this.FilePaths.Contains(path) || this.FilePaths.Count >= 9)
            {
                return;
            }

            this.FilePaths.Add(path);
            App.DockPanel.Reload();
        }

        public void RemoveApplication(string path)
        {
            this.FilePaths.Remove(path);
            App.DockPanel.Reload();
        }

        public static void IconStackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            int index = App.DockPanel.ContentPanel.Children.IndexOf((StackPanel)sender);
            string path = App.DockPanel.Property.FilePaths[index];
            DirectoryInfo? info = Directory.GetParent(path);
            if (Keyboard.IsKeyDown(Key.R) || !File.Exists(path))
            {
                App.DockPanel.Property.RemoveApplication(path);
            }
            else if (Keyboard.IsKeyDown(Key.LeftCtrl) && info is not null)
                Process.Start("explorer.exe", info.ToString());
            else
                Process.Start("explorer.exe", path);
        }
    }
}
