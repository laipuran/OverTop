using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static OverTop.App;
using static OverTop.API;

namespace OverTop.Floatings
{
    /// <summary>
    /// WeatherWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WeatherWindow : Window
    {
        public static bool changeWeather = false;

        public WeatherWindow(WeatherWindowProperty property)
        {
            InitializeComponent();

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
            WeatherInformation wi = GetWeatherInformation(settings.i2.adcode);

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
    }
}
