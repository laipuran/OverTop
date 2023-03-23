using Newtonsoft.Json;
using OverTop.ContentWindows;
using PuranLai.Algorithms;
using PuranLai.Tools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static OverTop.AppWindowOps;
using static OverTop.CommonWindowOps;
using Point = System.Windows.Point;

namespace OverTop
{
    public static class ExtentedWindowOps
    {
        public static void ToBottom(this Window window)
        {
            CommonWindowOps.SetWindowPos(new WindowInteropHelper(window).Handle,
                (IntPtr)WindowZIndex.Bottom, (int)window.Left,
                (int)window.Top, (int)window.Width, (int)window.Height, 0);
        }

        public static void ToTop(this Window window)
        {
            CommonWindowOps.SetWindowPos(new WindowInteropHelper(window).Handle,
                (IntPtr)WindowZIndex.TopMost, (int)window.Left,
                (int)window.Top, (int)window.Width, (int)window.Height, 0);
        }

        public static void Save(this DockWindow window)
        {
            AppWindowProperty appWindow = window.Property;
            string json = JsonConvert.SerializeObject(appWindow);
            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\OverTop\\AppWindow.json";
            DirectoryInfo? info = Directory.GetParent(filePath);
            if (info is not null)
            {
                Directory.CreateDirectory(info.FullName);
                File.WriteAllText(filePath, json);
            }
        }

        public static Point GetMiddlePoint(this Window window)
        {
            Point point = new();
            point.X = window.Left + window.Width / 2;
            point.Y = window.Top + window.Height / 2;

            return point;
        }

        public static AppWindowOps.ScreenPart GetPart(this Point point)
        {
            // Can automatically change when dpi is changed
            Point screenSize = new();
            screenSize.X = SystemParameters.PrimaryScreenWidth;
            screenSize.Y = SystemParameters.PrimaryScreenHeight;

            if (point.X < screenSize.X / 4)
            {
                return ScreenPart.LeftPart;
            }
            else if (point.X > screenSize.X * 3 / 4)
            {
                return ScreenPart.RightPart;
            }
            else if (point.Y < screenSize.Y / 2)
            {
                return ScreenPart.TopPart;
            }
            else
            {
                return ScreenPart.BottomPart;
            }
        }

        public static void SetWindowPos(this DockWindow window, ScreenPart part)
        {
            if (part == ScreenPart.TopPart)
            {
                window.Height = 60;
                window.Width = 560;
                window.ContentStackPanel.Orientation = System.Windows.Controls.Orientation.Horizontal;
                window.Top = 0;
                window.Left = (SystemParameters.FullPrimaryScreenWidth - window.Width) / 2;
                window.Property.screenPart = ScreenPart.TopPart;
            }
            else if (part == ScreenPart.BottomPart)
            {
                window.Height = 60;
                window.Width = 560;
                window.ContentStackPanel.Orientation = System.Windows.Controls.Orientation.Horizontal;
                window.Top = SystemParameters.FullPrimaryScreenHeight - window.Height;
                window.Left = (SystemParameters.FullPrimaryScreenWidth - window.Width) / 2;
                window.Property.screenPart = ScreenPart.BottomPart;
            }
            else if (part == ScreenPart.LeftPart)
            {
                window.Height = 560;
                window.Width = 60;
                window.ContentStackPanel.Orientation = System.Windows.Controls.Orientation.Vertical;
                window.Top = (SystemParameters.FullPrimaryScreenHeight - window.Height) / 2;
                window.Left = 0;
                window.Property.screenPart = ScreenPart.LeftPart;
            }
            else
            {
                window.Height = 560;
                window.Width = 60;
                window.ContentStackPanel.Orientation = System.Windows.Controls.Orientation.Vertical;
                window.Top = (SystemParameters.FullPrimaryScreenHeight - window.Height) / 2;
                window.Left = SystemParameters.FullPrimaryScreenWidth - window.Width;
                window.Property.screenPart = ScreenPart.RightPart;
            }
        }

        public static void Reload(this Window currentWindow)
        {
            #region If CustomWindow
            if (currentWindow is CustomWindow)
            {
                CustomWindow window = (CustomWindow)currentWindow;
                HangerWindowProperty property = window.Property;
                window.Top = property.Top == 0 ? window.Top : property.Top;
                window.Left = property.Left == 0 ? window.Left : property.Left;
                window.Width = property.Width;
                window.Height = property.Height;
                System.Drawing.Color tempColor = ColorTranslator.FromHtml(property.HtmlColor);
                System.Windows.Media.Color color = System.Windows.Media.Color.FromRgb(tempColor.R, tempColor.G, tempColor.B);
                window.Background = new SolidColorBrush(color);
                window.Title = property.Guid;

                window.ContentPanel.Children.Clear();
                foreach (KeyValuePair<HangerWindowProperty.ContentType, string> pair in property.Contents)
                {
                    if (pair.Key == HangerWindowProperty.ContentType.Text)
                    {
                        TextBlock newTextBlock = new();
                        newTextBlock.Style = (Style)window.FindResource("ContentTextBlockStyle");
                        newTextBlock.Text = pair.Value;
                        StackPanel TextStackPanel = new();
                        TextStackPanel.Children.Add(newTextBlock);
                        TextStackPanel.MouseLeftButtonDown += HangerWindowOps.TextPanel_MouseLeftButtonDown;
                        window.ContentPanel.Children.Add(TextStackPanel);
                    }
                    else if (pair.Key == HangerWindowProperty.ContentType.Image)
                    {
                        try
                        {
                            MemoryStream ms = new(Convert.FromBase64String(pair.Value));
                            Bitmap bitmap = new(ms);
                            ms.Close();

                            System.Windows.Controls.Image newImage = new()
                            {
                                Source = Imaging.CreateBitmapSourceFromHBitmap(
                                    bitmap.GetHbitmap(), IntPtr.Zero,
                                    Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()),
                            };
                            StackPanel ImageStackPanel = new();
                            ImageStackPanel.Children.Add(newImage);
                            ImageStackPanel.MouseLeftButtonDown += HangerWindowOps.ImagePanel_MouseRightButtonDown;
                            window.ContentPanel.Children.Add(ImageStackPanel);
                        }
                        catch
                        {
                            MessageBox.Show("Image not exists!", "Over Top");
                        }
                    }
                }
            }
            #endregion
            #region If RecentWindow
            else if (currentWindow is RecentWindow)
            {
                RecentWindow window = (RecentWindow)currentWindow;
                RecentWindowProperty property = window.Property;
                window.Top = property.Top == 0 ? window.Top : property.Top;
                window.Left = property.Left == 0 ? window.Left : property.Left;
                window.Width = property.Width;
                window.Height = property.Height;
                System.Drawing.Color tempColor = ColorTranslator.FromHtml(property.HtmlColor);
                System.Windows.Media.Color color = System.Windows.Media.Color.FromRgb(tempColor.R, tempColor.G, tempColor.B);
                window.Background = new SolidColorBrush(color);
            }
            #endregion
            #region If DockWindow
            else if (currentWindow is DockWindow)
            {
                DockWindow window = (DockWindow)currentWindow;
                AppWindowProperty property = window.Property;
                if (property.FilePaths.Count != 0)
                {
                    window.ContentPanel.Children.Clear();
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

                            System.Windows.Controls.Image image = new()
                            {
                                Source = Imaging.CreateBitmapSourceFromHBitmap(
                                bitmap.GetHbitmap(), IntPtr.Zero,
                                Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()),
                                Stretch = Stretch.Fill,
                                Width = 40,
                                Height = 40
                            };
                            StackPanel IconStackPanel = new()
                            {
                                Margin = new(7.5, 7.5, 7.5, 7.5),
                                ToolTip = path,
                                HorizontalAlignment = HorizontalAlignment.Center,
                                VerticalAlignment = VerticalAlignment.Center,
                            };
                            IconStackPanel.Children.Add(image);
                            IconStackPanel.MouseLeftButtonDown += AppWindowProperty.IconStackPanel_MouseLeftButtonDown;
                            IconStackPanel.MouseEnter += IconStackPanel_MouseEnter;
                            IconStackPanel.MouseLeave += IconStackPanel_MouseLeave;
                            window.ContentPanel.Children.Add(IconStackPanel);
                        }
                        catch { }
                    }
                }
                window.SetWindowPos(property.screenPart);
            }
            #endregion
        }

        private static void IconStackPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            StackPanel IconStackPanel = (StackPanel)sender;
            System.Windows.Controls.Image image = (System.Windows.Controls.Image)IconStackPanel.Children[0];
            Action<double> SetPanelWidth = new((double value) =>
            {
                double width = value;
                Action<double> set = new((double value) => { image.Width = value; });
                image.Dispatcher.Invoke(set, width);
            });
            Action<double> SetPanelHeight = new((double value) =>
            {
                double height = value;
                Action<double> set = new((double value) => { image.Height = value; });
                image.Dispatcher.Invoke(set, height);
            });
            unsafe
            {
                AnimationPool pool = new();
                pool.Add(100, image.Width, 40, Animation.GetSineValue, SetPanelWidth);
                pool.Add(100, image.Height, 40, Animation.GetSineValue, SetPanelHeight);
                pool.StartAllAnimations();
            }
        }

        private static void IconStackPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            StackPanel IconStackPanel = (StackPanel)sender;
            System.Windows.Controls.Image image = (System.Windows.Controls.Image)IconStackPanel.Children[0];
            Action<double> SetPanelWidth = new((double value) =>
            {
                double width = value;
                Action<double> set = new((double value) => { image.Width = value; });
                image.Dispatcher.Invoke(set, width);
            });
            Action<double> SetPanelHeight = new((double value) =>
            {
                double height = value;
                Action<double> set = new((double value) => { image.Height = value; });
                image.Dispatcher.Invoke(set, height);
            });
            unsafe
            {
                AnimationPool pool = new();
                pool.Add(100, image.Width, 50, Animation.GetLinearValue, SetPanelWidth);
                pool.Add(100, image.Height, 50, Animation.GetLinearValue, SetPanelHeight);
                pool.StartAllAnimations();
            }
        }
    }

    public class CommonWindowOps
    {
        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndlnsertAfter, int X, int Y, int cx, int cy, uint Flags);

        public enum WindowType
        {
            Hanger = 0,
            Recent = 1
        }

        public enum WindowZIndex
        {
            Top = 0,
            Bottom = 1,
            TopMost = -1,
            NoTopMost = -2,
        }

        public static void ChangeZIndex(bool isTop, Window window)
        {
            if (isTop)
            {
                window.ToTop();
            }
            else
            {
                window.ToBottom();
            }
        }

        public static bool ChangeStatus(bool isTop, Window window)
        {
            if (isTop)
            {
                window.ToBottom();
                window.Topmost = false;
            }
            else
            {
                window.ToTop();
                window.Topmost = true;
            }
            return !isTop;
        }
    }

    public class HangerWindowOps
    {
        public static void ImagePanel_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            StackPanel currentStackPanel = (StackPanel)sender;
            StackPanel ContentPanel = (StackPanel)currentStackPanel.Parent;
            CustomWindow currentWindow = GetHostWindow(currentStackPanel);
            int index = ContentPanel.Children.IndexOf(currentStackPanel);
            if (Keyboard.IsKeyDown(Key.R))
            {
                currentWindow.Property.Contents.RemoveAt(index);
            }
            currentWindow.Reload();
        }

        public static void TextPanel_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            StackPanel currentStackPanel = (StackPanel)sender;
            StackPanel ContentPanel = (StackPanel)currentStackPanel.Parent;
            TextBlock currentTextBlock = (TextBlock)currentStackPanel.Children[0];
            CustomWindow currentWindow = GetHostWindow(currentStackPanel);
            int index = ContentPanel.Children.IndexOf(currentStackPanel);
            if (Keyboard.IsKeyDown(Key.M))
            {
                TextWindow textWindow = new("请输入文本：", currentTextBlock.Text);
                textWindow.ShowDialog();
                if (textWindow.result is null)
                {
                    return;
                }
                currentWindow.Property.Contents[index] = new(HangerWindowProperty.ContentType.Text, textWindow.result);

            }
            else if (Keyboard.IsKeyDown(Key.R))
            {
                currentWindow.Property.Contents.RemoveAt(index);
            }
            currentWindow.Reload();
        }

        public static CustomWindow GetHostWindow(StackPanel currentPanel)
        {
            StackPanel ContentPanel = (StackPanel)currentPanel.Parent;
            StackPanel OriginStackPanel = (StackPanel)ContentPanel.Parent;
            ScrollViewer Scroller = (ScrollViewer)OriginStackPanel.Parent;
            return (CustomWindow)Scroller.Parent;
        }
    }

    public class AppWindowOps
    {
        public enum Quadrant // not used
        {
            Quadrant1,
            Quadrant2,
            Quadrant3,
            Quadrant4
        }

        public enum ScreenPart
        {
            TopPart,
            BottomPart,
            RightPart,
            LeftPart
        }

        public static Quadrant GetQuadrant(Point point) // not used
        {
            Point middleScreen = new();
            middleScreen.X = SystemParameters.PrimaryScreenWidth / 2;
            middleScreen.Y = SystemParameters.PrimaryScreenHeight / 2;
            if (point.X > middleScreen.X && point.Y < middleScreen.Y)
            {
                return Quadrant.Quadrant1;
            }
            else if (point.X < middleScreen.X && point.Y < middleScreen.Y)
            {
                return Quadrant.Quadrant2;
            }
            else if (point.X < middleScreen.X && point.Y > middleScreen.Y)
            {
                return Quadrant.Quadrant3;
            }
            else // if (point.X > middleScreen.X && point.Y > middleScreen.Y)
            {
                return Quadrant.Quadrant4;
            }
        }
    }
}
