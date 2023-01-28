using Newtonsoft.Json;
using OverTop.Floatings;
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
using Image = System.Windows.Controls.Image;
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

        public static void Save(this AppWindow window)
        {
            AppWindowProperty appWindow = new();
            appWindow.screenPart = window.GetMiddlePoint().GetPart();
            foreach (StackPanel item in window.ContentStackPanel.Children)
            {
                string? path = item.ToolTip.ToString();
                if (path is not null)
                    appWindow.FilePaths.Add(path);
            }
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

        public static void SetWindowPos(this AppWindow window, ScreenPart part)
        {
            if (part == ScreenPart.TopPart)
            {
                window.Height = 60;
                window.Width = 560;
                window.ContentStackPanel.Orientation = System.Windows.Controls.Orientation.Horizontal;
                window.Top = 0;
                window.Left = (SystemParameters.FullPrimaryScreenWidth - window.Width) / 2;
            }
            else if (part == ScreenPart.BottomPart)
            {
                window.Height = 60;
                window.Width = 560;
                window.ContentStackPanel.Orientation = System.Windows.Controls.Orientation.Horizontal;
                window.Top = SystemParameters.FullPrimaryScreenHeight - window.Height;
                window.Left = (SystemParameters.FullPrimaryScreenWidth - window.Width) / 2;
            }
            else if (part == ScreenPart.LeftPart)
            {
                window.Height = 560;
                window.Width = 60;
                window.ContentStackPanel.Orientation = System.Windows.Controls.Orientation.Vertical;
                window.Top = (SystemParameters.FullPrimaryScreenHeight - window.Height) / 2;
                window.Left = 0;
            }
            else
            {
                window.Height = 560;
                window.Width = 60;
                window.ContentStackPanel.Orientation = System.Windows.Controls.Orientation.Vertical;
                window.Top = (SystemParameters.FullPrimaryScreenHeight - window.Height) / 2;
                window.Left = SystemParameters.FullPrimaryScreenWidth - window.Width;
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

        public static void ReloadWindow(Window window, ContentWindowProperty property)
        {
            window.Top = property.top == 0 ? window.Top : property.top;
            window.Left = property.left == 0 ? window.Left : property.left;
            window.Width = property.width;
            window.Height = property.height;
            System.Drawing.Color tempColor = ColorTranslator.FromHtml(property.backgroundColor);
            System.Windows.Media.Color color = System.Windows.Media.Color.FromRgb(tempColor.R, tempColor.G, tempColor.B);
            window.Background = new SolidColorBrush(color);
            if (window is HangerWindow)
            {
                window.Title = ((HangerWindowProperty)property).guid;
                ((HangerWindow)window).ContentPanel.Children.RemoveRange(0, ((HangerWindow)window).ContentPanel.Children.Count);
                foreach (KeyValuePair<HangerWindowProperty.ContentType, string> pair in ((HangerWindowProperty)property).contents)
                {
                    if (pair.Key == HangerWindowProperty.ContentType.Text)
                    {
                        TextBlock newTextBlock = new();
                        newTextBlock.Style = (Style)window.FindResource("ContentTextBlockStyle");
                        newTextBlock.Text = pair.Value;
                        StackPanel TextStackPanel = new();
                        TextStackPanel.Children.Add(newTextBlock);
                        TextStackPanel.MouseLeftButtonDown += TextPanel_MouseLeftButtonDown;
                        ((HangerWindow)window).ContentPanel.Children.Add(TextStackPanel);
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
                            ImageStackPanel.MouseLeftButtonDown += ImagePanel_MouseRightButtonDown;
                            ((HangerWindow)window).ContentPanel.Children.Add(ImageStackPanel);
                        }
                        catch
                        {
                            throw new CustomException("Image not exists!");
                        }
                    }
                }
            }
        }

        private static void ImagePanel_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            StackPanel currentStackPanel = (StackPanel)sender;
            Image currentImage = (Image)currentStackPanel.Children[0];
            HangerWindow currentWindow = (HangerWindow)((ScrollViewer)((StackPanel)currentStackPanel.Parent).Parent).Parent;
            if (Keyboard.IsKeyDown(Key.R))
            {
                BitmapSource source = (BitmapSource)currentImage.Source;
                MemoryStream stream1 = new();
                BmpBitmapEncoder encoder = new();
                encoder.Frames.Add(BitmapFrame.Create(source));
                encoder.Save(stream1);
                Bitmap image = new(stream1);
                stream1.Close();

                List<KeyValuePair<HangerWindowProperty.ContentType, string>> list = new();
                list.AddRange(currentWindow.Property.contents);
                foreach (var pair in list)
                {
                    if (pair.Key == HangerWindowProperty.ContentType.Text)
                        continue;

                    MemoryStream ms = new(Convert.FromBase64String(pair.Value));
                    Bitmap bitmap = new(ms);
                    ms.Close();
                    bool flag = true;
                    for (int i = 0; i < bitmap.Width; i++)
                    {
                        if (!flag)
                            break;
                        for (int j = 0; j < bitmap.Height; j++)
                        {
                            if (!flag)
                                break;
                            if (bitmap.GetPixel(i, j) != (bitmap.GetPixel(i, j)))
                            {
                                flag = false;
                                break;
                            }
                        }
                    }
                    if (flag)
                    {
                        currentWindow.Property.contents.Remove(pair);
                    }
                }
            }
            ReloadWindow(currentWindow, currentWindow.Property);
        }

        public static void TextPanel_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            StackPanel currentStackPanel = (StackPanel)sender;
            TextBlock currentTextBlock = (TextBlock)currentStackPanel.Children[0];
            HangerWindow currentWindow = (HangerWindow)((ScrollViewer)((StackPanel)currentStackPanel.Parent).Parent).Parent;
            if (Keyboard.IsKeyDown(Key.M))
            {
                int index = currentWindow.Property.contents.IndexOf(new(HangerWindowProperty.ContentType.Text, currentTextBlock.Text));
                TextWindow textWindow = new("请输入文本：", currentTextBlock.Text);
                textWindow.ShowDialog();
                if (textWindow.result is null)
                {
                    return;
                }
                currentWindow.Property.contents[index] = new(HangerWindowProperty.ContentType.Text, textWindow.result);

            }
            else if (Keyboard.IsKeyDown(Key.R))
            {
                currentWindow.Property.contents.Remove(new(HangerWindowProperty.ContentType.Text, currentTextBlock.Text));
            }
            ReloadWindow(currentWindow, currentWindow.Property);
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
