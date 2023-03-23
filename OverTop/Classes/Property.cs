using OverTop.ContentWindows;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
        public System.Windows.Media.Color BackgroundColor;
        public int Width;
        public int Height;
    }

    public class WeatherWindowProperty : IProperty
    {
        public double Left;
        public double Top;
        public bool Visibility;

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
            property.Left = 30;
            property.Top = 30;
            property.Visibility = false;
            return property;
        }
    }

    public class ContentWindowProperty
    {
        public string HtmlColor = "";
        public int Width;
        public int Height;
        public double Left;
        public double Top;
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
            this.Width = property.Width;
            this.Height = property.Height;
            System.Windows.Media.Color tempColor = property.BackgroundColor;
            this.HtmlColor = System.Drawing.ColorTranslator.ToHtml(System.Drawing.Color.FromArgb(tempColor.R, tempColor.G, tempColor.B));
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
        public List<KeyValuePair<ContentType, string>> Contents = new();
        public string Guid = "";
        public string Link = "";

        public enum ContentType
        {
            Text,
            Image
        }

        public void FromProperty(WindowProperty property)
        {
            this.Width = property.Width;
            this.Height = property.Height;
            System.Windows.Media.Color tempColor = property.BackgroundColor;
            this.HtmlColor = System.Drawing.ColorTranslator.ToHtml(System.Drawing.Color.FromArgb(tempColor.R, tempColor.G, tempColor.B));
        }

        public static HangerWindowProperty GetDefaultProperty()
        {
            HangerWindowProperty property = new();
            property.FromProperty(App.AppSettings.HangerWindowSettings);
            property.Guid = System.Guid.NewGuid().ToString();
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
