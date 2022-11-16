﻿using Newtonsoft.Json;
using OverTop.Floatings;
using OverTop.Pages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using static OverTop.AppWindowOps;
using static OverTop.CommonWindowOps;

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
        public static string ip = "";
        public MainWindow()
        {
            InitializeComponent();
            App.mainWindow = this;
            
            FloatingListBoxItem.IsSelected = true;
            TitleTextBlock.Text = "浮窗控制面板";
            ContentFrame.NavigationService.Navigate(FloatingUri);

            Icon = GetIcon("icon");
            BackContentImage.Source = GetIcon("Back");
            MenuContentImage.Source = GetIcon("Menu");
            FloatingsContentImage.Source = GetIcon("Floatings");
            PropertiesContentImage.Source = GetIcon("Properties");

            Pages.StaticPropertyPage.ColorChanged();
            App.appWindow.Show();
        }

        public static ImageSource GetIcon(string name)
        {
            ResourceManager Loader = OverTop.Resources.ResourcesFile.ResourceManager;
#pragma warning disable CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
#pragma warning disable CS8604 // 引用类型参数可能为 null。
            Bitmap icon = new((System.Drawing.Image)Loader.GetObject(name));
#pragma warning restore CS8604 // 引用类型参数可能为 null。
#pragma warning restore CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
            return Imaging.CreateBitmapSourceFromHBitmap(icon.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
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
                App.appWindow.Save();

                int RecentWindows = 0, HangerWindows = 0;
                App.settings.HangerWindows = new();
                foreach (Window window in FloatingPanelPage.windows)
                {
                    if (window is RecentWindow)
                    {
                        RecentWindows++;
                        App.settings.RecentWindow = ((RecentWindow)window).Save();
                    }
                    else if (window is HangerWindow)
                    {
                        HangerWindowProperty? property = ((HangerWindow)window).Save();
                        if (property is null)
                        {
                            continue;
                        }
                        HangerWindows++;
                        App.settings.HangerWindows.Add(property);
                    }
                }
                if (RecentWindows == 0) App.settings.RecentWindow = null;
                if (HangerWindows == 0) App.settings.HangerWindows = null;
                App.settings.WeatherWindow = App.weatherWindow.Save();

                App.Current.Shutdown();
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ContentFrame.CanGoBack)
            {
                return;
            }
            ContentFrame.GoBack();
        }

        private void ContentFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if ("/" + ContentFrame.Source.ToString() == PropertyUri.ToString())
            {
                TitleTextBlock.Text = "系统动态属性";
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
