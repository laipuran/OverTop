using OverTop.Floatings;
using OverTop.Pages;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Resources;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
                Task.Run(() => Dispatcher.BeginInvoke(new Action(() => { MenuOpen(); })));
            }
            else
            {
                Task.Run(() => Dispatcher.BeginInvoke(new Action(() => { MenuClose(); })));
            }
            MenuClosed = !MenuClosed;
        }

        private async void MenuOpen()
        {
            DateTime start = DateTime.Now;
            TimeSpan span = new();
            double k = (170 - 45) / Math.Sin(GetX(200, 150));
            while (span.TotalMilliseconds < 200)
            {
                span = DateTime.Now - start;
                await Task.Run(() => Dispatcher.BeginInvoke(new Action(() =>
                {
                    double x = span.TotalMilliseconds;
                    double value = Math.Sin(GetX(x, 150)) * k + 45;
                    MenuStackPanel.Width = value;
                })));
            }
            MenuStackPanel.Width = 170;
        }

        private async void MenuClose()
        {
            DateTime start = DateTime.Now;
            TimeSpan span = new();
            double k = (170 - 45) / Math.Sin(GetX(200, 150));
            while (span.TotalMilliseconds < 200)
            {
                span = DateTime.Now - start;
                await Task.Run(() => Dispatcher.BeginInvoke(new Action(() =>
                {
                    double x = span.TotalMilliseconds;
                    double value = 170 - Math.Sin(GetX(x, 150)) * k;
                    MenuStackPanel.Width = value;
                })));
            }
            MenuStackPanel.Width = 45;
        }

        private static double GetX(double time, int T)
        {
            double x = Math.PI * time / T / 2;

            return x;
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

        private async void WhenCloseAnimation()
        {
            Topmost = true;
            int time = 500;
            double speedX = Left / time;
            double speedY = Top / time;
            double speedWidth = Width / time;
            double speedHeight = Height / time;
            double speedOpacity = 1.00 / time;
            double left = Left;
            double top = Top;
            double width = Width;
            double height = Height;
            double opacity = 1;
            DateTime start = DateTime.Now;
            TimeSpan span = new();
            Debug.WriteLine("START: " + span.TotalMilliseconds + " L: " + Left + " Top: " + Top + " W: " + Width + " H: " + Height + " O: " + Opacity);
            while (span.TotalMilliseconds < time)
            {
                span = DateTime.Now - start;
                await Task.Run(() => Dispatcher.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        double l = left - span.TotalMilliseconds * speedX;
                        double t = top - span.TotalMilliseconds * speedY;
                        double w = width - span.TotalMilliseconds * speedWidth;
                        double h = height - span.TotalMilliseconds * speedHeight;
                        double o = opacity - span.TotalMilliseconds * speedOpacity;
                        Debug.WriteLine("T: " + Math.Round(span.TotalMilliseconds, 2) + " L: " + Math.Round(l, 2) + " Top: " + Math.Round(t, 2) + " W: " + Math.Round(w, 2) + " H: " + Math.Round(h, 2) + " O: " + Math.Round(o, 2));
                        Left = l;
                        Top = t;
                        Width = w;
                        Height = h;
                        Opacity = o;
                    }
                    catch { }
                })));
            }
            Topmost = false;
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

        private async void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                WhenCloseAnimation();
                await Task.Delay(500);
                this.Visibility = Visibility.Collapsed;
            }
        }
    }
}
