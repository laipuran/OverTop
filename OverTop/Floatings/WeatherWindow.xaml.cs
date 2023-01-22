using OverTop.Pages;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static OverTop.API;
using static OverTop.App;
using static PuranLai.Tools.ExtendedWindowOps;

namespace OverTop.Floatings
{
    /// <summary>
    /// WeatherWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WeatherWindow : Window
    {
        public bool isMouseIn = false;
        public static bool changeWeather = false;

        public WeatherWindow(WeatherWindowProperty property)
        {
            InitializeComponent();

            FloatingPanelPage.windows.Add(this);

            this.Top = property.top == 0 ? Top : property.top;
            this.Left = property.left == 0 ? Left : property.left;

        }

        private void WeatherStackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ShowWeatherOnce();
        }

        private async void ShowWeather()
        {
            while (changeWeather)
            {
                ShowWeatherOnce();
                await Task.Delay(10 * 60 * 1000);
            }
        }

        private void ShowWeatherOnce()
        {
            if (App.settings.ip is null || settings.i2 is null)
            {
                this.Visibility = Visibility.Collapsed;
                return;
            }
            WeatherInformation? wi = GetWeatherInformation(settings.i2.adcode);

            if (wi is null)
            {
                WeatherTextBlock.Text = "NS00001";
                return;
            }

            WeatherTextBlock.Text = wi.lives[0].weather +
                " " + wi.lives[0].winddirection +
                " " + wi.lives[0].windpower + "级";
            TempTextBlock.Text = wi.lives[0].temperature + "℃";
            LocationTextBlock.Text = settings.i2.province + "\n" + settings.i2.city;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            changeWeather = true;
            await Task.Run(() => Dispatcher.BeginInvoke(new Action(() => { ShowWeather(); })));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            changeWeather = false;
            Visibility = Visibility.Collapsed;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ShowWeatherOnce();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                switch (e.Key)
                {
                    case Key.Left: Left -= 1; break;
                    case Key.Right: Left += 1; break;
                    case Key.Up: Top -= 1; break;
                    case Key.Down: Top += 1; break;
                    case Key.Escape: Close(); break;
                    default: break;
                }
            }
            catch { }
        }

        private unsafe void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            fixed (bool* MouseIn = &isMouseIn)
            {
                this.ChangeOpacity(OpacityOptions._75, MouseIn);
            }
        }

        private unsafe void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            fixed (bool* MouseIn = &isMouseIn)
            {
                this.ChangeOpacity(OpacityOptions._25, MouseIn);
            }
        }
    }
}
