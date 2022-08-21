using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using static OverTop.HangerWindowClass;
using static OverTop.WeatherClass;

namespace OverTop.Floatings
{
    /// <summary>
    /// HangerWindow.xaml 的交互逻辑
    /// </summary>
    public partial class HangerWindow : Window
    {
        [DllImport("user32.dll")]

        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndlnsertAfter, int X, int Y, int cx, int cy, uint Flags);
        private static bool isBottom = false;
        public static bool showWeather = false;

        public HangerWindow()
        {
            InitializeComponent();
        }
        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (System.Windows.Input.Mouse.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                Close();
            }
            else if (e.Key == System.Windows.Input.Key.Tab)
            {
                isBottom = WindowClass.ChangeStatus(isBottom, this);
            }
        }

        private void ContentStackPanel_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.LeftCtrl))
            {
                SaveWindow(this);
            }
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Window_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            WindowClass.ChangeZIndex(isBottom, this);
            Scroller.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
        }

        private void Window_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            WindowClass.ChangeZIndex(isBottom, this);
            Scroller.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
        }

        private void Window_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            App.currentWindow = this;
            App.contentStackPanel = ContentStackPanel;
            App.windowType = WindowClass.WindowType.Hanger;
            Window propertyWindow = new PropertyWindow
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            propertyWindow.ShowDialog();
            Width = App.parameterClass.width;
            Height = App.parameterClass.height;
            Background = new SolidColorBrush(App.parameterClass.backGroundColor);
            Opacity = App.parameterClass.alpha;
        }

        private void WeatherStackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        //    if (showWeather)
        //    {
        //        showWeather = false;
        //        WeatherStackPanel.Visibility = Visibility.Collapsed;
        //    }
        //    else
        //    {
        //        showWeather = true;
        //        WeatherStackPanel.Visibility = Visibility.Visible;
        //        Task.Run(() => Dispatcher.BeginInvoke(new Action(() => { ShowWeather(); })));
        //    }
        }

        private async void ShowWeather()
        {
            while (showWeather)
            {
                ShowWeatherOnce();
                await Task.Delay(10 * 60 * 1000);
            }
        }

        private void ShowWeatherOnce()
        {
            IpInformation i2 = GetIpInformation(MainWindow.ip);
            WeatherInformation wi = GetWeatherInformation(i2.adcode);

            WeatherTextBlock.Text = wi.lives[0].weather +
                " " + wi.lives[0].winddirection +
                " " + wi.lives[0].windpower + "级";
            TempTextBlock.Text = wi.lives[0].temperature + "℃";
            LocationTextBlock.Text = i2.province + "\n" + i2.city;
        }
    }
}
