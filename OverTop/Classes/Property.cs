using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static OverTop.AppWindowOps;

namespace OverTop.Floatings
{
    interface IProperty
    {
        Window GetWindow();
        void FromProperty(WindowProperty property);
    }

    public class WindowProperty
    {
        public System.Windows.Media.Color backGroundColor;
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
            System.Windows.Media.Color tempColor = property.backGroundColor;
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

        public enum ContentType
        {
            Text,
            Image
        }

        public void FromProperty(WindowProperty property)
        {
            this.width = property.width;
            this.height = property.height;
            System.Windows.Media.Color tempColor = property.backGroundColor;
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
            return new HangerWindow(this);
            // TODO: Not understanding this if structure
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
            return new AppWindow(this);
        }

        public void FromProperty(WindowProperty property)
        {
            // Emplty
        }

        public static AppWindowProperty GetDefaultProperty()
        {
            return new();
        }

        public void AddFile(string path)
        {
            if (this.FilePaths.Contains(path) || this.FilePaths.Count >= 9)
            {
                return;
            }

            this.FilePaths.Add(path);
            ReloadWindow(App.AppWindow);
        }

        public void ReloadWindow(AppWindow window)
        {
            AppWindowProperty property = window.Property;
            if (property is not null)
            {
                window.ContentPanel = new();
                foreach (string path in property.FilePaths)
                {
                    try
                    {
                        Icon? icon = System.Drawing.Icon.ExtractAssociatedIcon(path);
                        Bitmap bitmap;
                        if (icon is null)
                            continue;
                        else
                            bitmap = icon.ToBitmap();

                        System.Windows.Controls.Image image = new();
                        image.Source = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                        image.Width = 40;
                        Thickness margin = new(10, 10, 10, 10);
                        StackPanel IconStackPanel = new()
                        {
                            Margin = margin,
                            ToolTip = path,
                            AllowDrop = true,
                        };
                        IconStackPanel.Children.Add(image);
                        IconStackPanel.MouseLeftButtonDown += IconStackPanel_MouseLeftButtonDown;
                        App.AppWindow.ContentPanel.Children.Add(IconStackPanel);
                    }
                    catch
                    {
                        continue;
                    }
                }
                window.SetWindowPos(property.screenPart);
            }
        }

        public void IconStackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string? path = ((StackPanel)sender).ToolTip.ToString();
            if (path is null)
                return;

            DirectoryInfo? info = Directory.GetParent(path);
            if (Keyboard.IsKeyDown(Key.R) || !File.Exists(path))
            {
                this.FilePaths.Remove(path);
                ((StackPanel)((StackPanel)sender).Parent).Children.Remove(sender as UIElement);
            }
            else if (Keyboard.IsKeyDown(Key.LeftCtrl) && info is not null)
            {
                Process.Start("explorer.exe", info.ToString());
            }
            else
                Process.Start("explorer.exe", path);
        }
    }
}
