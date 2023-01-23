﻿using OverTop.Floatings;
using OverTop.Pages;
using PuranLai.Algorithms;
using PuranLai.Tools;
using System;
using System.Drawing;
using System.Resources;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Windows.Media.Color;

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
        public MainWindow()
        {
            InitializeComponent();
            App.MainWindow = this;

            FloatingListBoxItem.IsSelected = true;
            TitleTextBlock.Text = "浮窗控制面板";
            ContentFrame.NavigationService.Navigate(FloatingUri);

            Icon = GetIcon("icon");
            IconImage.Source = GetIcon("icon");
            BackContentImage.Source = GetIcon("Back");
            MenuContentImage.Source = GetIcon("Menu");
            FloatingsContentImage.Source = GetIcon("Floatings");
            PropertiesContentImage.Source = GetIcon("Properties");

            MinimizeImage.Source = GetIcon("Minimize");
            DisappearImage.Source = GetIcon("Disappear");
            CloseWindowImage.Source = GetIcon("Window_Close");

            Pages.StaticPropertyPage.ColorChanged();

            App.AppWindow.Show();
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
                Task.Run(() => Dispatcher.BeginInvoke(new Action(() => { MenuOpen(); })));
            }
            else
            {
                Task.Run(() => Dispatcher.BeginInvoke(new Action(() => { MenuClose(); })));
            }
            MenuClosed = !MenuClosed;
        }

        private unsafe void MenuOpen()
        {
            Action<double> SetPanelWidth = new((double value) =>
            {
                double width = value;
                Action<double> set = new((double value) => { MenuStackPanel.Width = value; });
                Dispatcher.Invoke(set, width);
            });
            fixed(bool* isOpened = &MenuClosed)
            {
                Animation open = new Animation(200, MenuStackPanel.Width, 160, Animation.GetSineValue, SetPanelWidth, 50, Flag: isOpened);
                open.StartAnimationAsync();
            }
        }

        private unsafe void MenuClose()
        {
            Action<double> SetPanelWidth = new((double value) =>
            {
                double width = value;
                Action<double> set = new((double value) => { MenuStackPanel.Width = value; });
                Dispatcher.Invoke(set, width);
            });
            fixed (bool* isOpened = &MenuClosed)
            {
                Animation open = new Animation(200, MenuStackPanel.Width, 45, Animation.GetSineValue, SetPanelWidth, 50, Flag: isOpened);
                open.StartAnimationAsync();
            }
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

        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("是否要退出 Over Top ？", "退出 Over Top", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.No)
            {
                e.Cancel = true;
                return;
            }
            else
            {
                App.AppWindow.Save();

                int RecentWindows = 0, HangerWindows = 0;
                App.AppSettings.HangerWindows = new();
                foreach (System.Windows.Window window in FloatingPanelPage.windows)
                {
                    if (window is RecentWindow)
                    {
                        RecentWindows++;
                        App.AppSettings.RecentWindow = ((RecentWindow)window).Save();
                    }
                    else if (window is HangerWindow)
                    {
                        HangerWindowProperty? property = ((HangerWindow)window).Save();
                        if (property is null)
                        {
                            continue;
                        }
                        HangerWindows++;
                        App.AppSettings.HangerWindows.Add(property);
                    }
                }
                if (RecentWindows == 0) App.AppSettings.RecentWindow = null;
                if (HangerWindows == 0) App.AppSettings.HangerWindows = null;
                App.AppSettings.WeatherWindow = App.WeatherWindow.Save();

                Disappear();
                await Task.Delay(500);
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
                TitleTextBlock.Text = "系统属性";
                PropertyListBoxItem.IsSelected = true;
            }
            else if ("/" + ContentFrame.Source.ToString() == FloatingUri.ToString())
            {
                TitleTextBlock.Text = "浮窗控制";
                FloatingListBoxItem.IsSelected = true;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                Disappear();
            }
        }

        private void DockPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private async void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            Disappear();
            await Task.Delay(500);
            this.WindowState = WindowState.Minimized;
        }

        private async void DisappearButton_Click(object sender, RoutedEventArgs e)
        {
            Disappear();
            await Task.Delay(500);
            Visibility = Visibility.Collapsed;
        }

        private void CloseWindowButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Disappear()
        {
            Action<double> SetOpacity = new((double value) =>
            {
                double opacity = value;
                Action<double> set = new((double value) =>
                {
                    Opacity = opacity;
                });
                Dispatcher.Invoke(set, opacity);
            });
            unsafe
            {
                Animation animation = new(500, Opacity, 0, Animation.GetLinearValue, SetOpacity);
                animation.StartAnimationAsync();
            }
        }

        private void Window_GotFocus(object sender, RoutedEventArgs e)
        {
            this.Opacity = 1;
        }
    }
}
