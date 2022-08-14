﻿using Newtonsoft.Json;
using OverTop.Floatings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Resources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using static OverTop.AppWindowClass;
using static OverTop.WindowClass;

namespace OverTop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool MenuClosed = true;
        Uri PropertyUri = new Uri("/Pages/StaticPropertyPage.xaml", UriKind.Relative);
        Uri FloatingUri = new Uri("/Pages/FloatingPanelPage.xaml", UriKind.Relative);
        AppWindow appWindow = new Floatings.AppWindow();
        public MainWindow()
        {
            InitializeComponent();
            App.mainWindow = this;

            FloatingListBoxItem.IsSelected = true;
            TitleTextBlock.Text = "浮窗控制面板";
            ContentFrame.NavigationService.Navigate(FloatingUri);

            Icon = GetIcon();

            Pages.StaticPropertyPage.ColorChanged();
            LoadAppWindow();
            GetSettingsFromFile();
        }

        private ImageSource GetIcon()
        {
            ResourceManager Loader = OverTop.Resources.ResourcesFile.ResourceManager;
#pragma warning disable CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
#pragma warning disable CS8604 // 引用类型参数可能为 null。
            Bitmap icon = new((System.Drawing.Image)Loader.GetObject("icon"));
#pragma warning restore CS8604 // 引用类型参数可能为 null。
#pragma warning restore CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
            return Imaging.CreateBitmapSourceFromHBitmap(icon.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }

        private void LoadAppWindow()
        {
            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\OverTop\\AppWindow.json";
            if (!File.Exists(filePath))
            {
                appWindow.Show();
                SetWindowPos(appWindow, ScreenPart.TopPart);
                return;
            }
            string json = File.ReadAllText(filePath);

#pragma warning disable CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
            AppWindowClass appWindowClass = JsonConvert.DeserializeObject<AppWindowClass>(json);
#pragma warning restore CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
#pragma warning disable CS8602 // 解引用可能出现空引用。
            foreach (string item in appWindowClass.filePath)
#pragma warning restore CS8602 // 解引用可能出现空引用。
            {
                try
                {
                    StackPanel appPanel = new();
                    System.Windows.Controls.Image image = new();
#pragma warning disable CS8602 // 解引用可能出现空引用。
                    Bitmap icon = System.Drawing.Icon.ExtractAssociatedIcon(item).ToBitmap();
#pragma warning restore CS8602 // 解引用可能出现空引用。
                    image.Source = System.Windows.Interop.Imaging
                        .CreateBitmapSourceFromHBitmap(icon.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    image.Width = 40;
                    Thickness margin = new(10, 10, 10, 10);
                    appPanel.Children.Add(image);
                    appPanel.Margin = margin;
                    appPanel.ToolTip = item;
                    appPanel.MouseLeftButtonDown += AppPanel_MouseLeftButtonDown;
                    appWindow.ContentStackPanel.Children.Add(appPanel);
                }
                catch { }
            }
            appWindow.Show();
            SetWindowPos(appWindow, appWindowClass.screenPart);
        }

        private void GetSettingsFromFile()
        {
            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\OverTop\\Settings.json";
            if (!File.Exists(filePath))
            {
                App.hangerSettingsClass.height = 150;
                App.hangerSettingsClass.width = 200;
                App.hangerSettingsClass.alpha = 0.7;
                App.hangerSettingsClass.backGroundColor = System.Windows.Media.Color.FromRgb(255, 161, 46);

                App.recentSettingsClass.height = 350;
                App.recentSettingsClass.width = 250;
                App.recentSettingsClass.alpha = 0.8;
                App.recentSettingsClass.backGroundColor = System.Windows.Media.Color.FromRgb(69, 181, 255);

                return;
            }
            string json = File.ReadAllText(filePath);
#pragma warning disable CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
            Dictionary<WindowType, PropertyClass> settings = JsonConvert.DeserializeObject<Dictionary<WindowType, PropertyClass>>(json);
#pragma warning restore CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
#pragma warning disable CS8602 // 解引用可能出现空引用。
            foreach (KeyValuePair<WindowType, PropertyClass> item in settings)
            {
                if (item.Key == WindowType.Hanger)
                {
                    App.hangerSettingsClass = item.Value;
                }
                else
                    App.recentSettingsClass = item.Value;
            }
#pragma warning restore CS8602 // 解引用可能出现空引用。
        }

        private void AppPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
#pragma warning disable CS8604 // 引用类型参数可能为 null。
            Process.Start("explorer.exe", ((StackPanel)sender).ToolTip.ToString());
#pragma warning restore CS8604 // 引用类型参数可能为 null。
        }

        private void SetWindowPos(AppWindow window, ScreenPart part)
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

        private void NavigationButton_Click(object sender, RoutedEventArgs e)
        {
            if (MenuClosed)
            {
                Storyboard openMenu = (Storyboard)FindResource("MenuOpen");
                openMenu.Begin();
            }
            else
            {
                Storyboard closeMenu = (Storyboard)FindResource("MenuClose");
                closeMenu.Begin();
            }
            MenuClosed = !MenuClosed;
        }

        private void ContentListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ContentListBox.SelectedItem == PropertyListBoxItem)
            {
                ContentFrame.NavigationService.Navigate(PropertyUri);
            }
            else if (ContentListBox.SelectedItem == FloatingListBoxItem)
            {
                ContentFrame.NavigationService.Navigate(FloatingUri);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("是否要退出 Over Top ？", "退出 Over Top", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.No)
            {
                e.Cancel = true;
                return;
            }
            else
            {
                SaveWindow(appWindow);
                App.Current.Shutdown();
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (ContentFrame.CanGoBack)
            {
                ContentFrame.GoBack();
                if ("/" + ContentFrame.Source.ToString() == PropertyUri.ToString())
                {
                    TitleTextBlock.Text = "系统静态属性";
                    PropertyListBoxItem.IsSelected = true;
                }
                else if ("/" + ContentFrame.Source.ToString() == FloatingUri.ToString())
                {
                    TitleTextBlock.Text = "浮窗控制面板";
                    FloatingListBoxItem.IsSelected = true;
                }
            }
        }

        private void ContentFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if ("/" + ContentFrame.Source.ToString() == PropertyUri.ToString())
            {
                TitleTextBlock.Text = "系统静态属性";
                PropertyListBoxItem.IsSelected = true;
            }
            else if ("/" + ContentFrame.Source.ToString() == FloatingUri.ToString())
            {
                TitleTextBlock.Text = "浮窗控制面板";
                FloatingListBoxItem.IsSelected = true;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                this.Visibility = Visibility.Collapsed;
            }
        }
    }
}
