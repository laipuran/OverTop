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
#pragma warning disable CS8604 // 引用类型参数可能为 null。
                appWindow.FilePaths.Add(item.ToolTip.ToString());
#pragma warning restore CS8604 // 引用类型参数可能为 null。
            }
            string json = JsonConvert.SerializeObject(appWindow);
            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\OverTop\\AppWindow.json";
#pragma warning disable CS8602 // 解引用可能出现空引用。
            Directory.CreateDirectory(Directory.GetParent(filePath).FullName);
#pragma warning restore CS8602 // 解引用可能出现空引用。
            File.WriteAllText(filePath, json);
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
                        StackPanel newStackPanel = new();
                        newStackPanel.Children.Add(newTextBlock);
                        newStackPanel.MouseLeftButtonDown += TextPanel_MouseLeftButtonDown;
                        ((HangerWindow)window).ContentPanel.Children.Add(newStackPanel);
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
                            StackPanel newStackPanel = new();
                            newStackPanel.Children.Add(newImage);
                            newStackPanel.MouseLeftButtonDown += NewStackPanel_MouseLeftButtonDown;
                            ((HangerWindow)window).ContentPanel.Children.Add(newStackPanel);
                        }
                        catch
                        {
                            throw new CustomException("Image not exists!");
                        }
                    }
                }
            }
        }

        private static void NewStackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.R))
            {
                ((StackPanel)((StackPanel)sender).Parent).Children.Remove((StackPanel)sender);
            }
        }

        public static void TextPanel_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            StackPanel currentStackPanel = (StackPanel)sender;
            TextBlock currentTextBlock = (TextBlock)currentStackPanel.Children[0];
            if (Keyboard.IsKeyDown(Key.M))
            {
                TextWindow textWindow = new("请输入文本：", currentTextBlock.Text);
                textWindow.ShowDialog();
                currentTextBlock.Text = textWindow.result;
            }
            else if (Keyboard.IsKeyDown(Key.R))
            {
                ((StackPanel)currentStackPanel.Parent).Children.Remove(currentStackPanel);
            }
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
