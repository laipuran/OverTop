﻿using System;
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
using static OverTop.AmapWeather;

namespace OverTop.Floatings
{
    /// <summary>
    /// WeatherWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WeatherWindow : Window
    {
        public static bool showWeather = false;
        public WeatherWindow()
        {
            InitializeComponent();
            Task.Run(() => Dispatcher.BeginInvoke(new Action(() => { ShowWeather(); })));
        }

        private void WeatherStackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ShowWeatherOnce();
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
            WeatherInformation wi = GetWeatherInformation(i2.adcode);

            WeatherTextBlock.Text = wi.lives[0].weather +
                " " + wi.lives[0].winddirection +
                " " + wi.lives[0].windpower + "级";
            TempTextBlock.Text = wi.lives[0].temperature + "℃";
            LocationTextBlock.Text = i2.province + "\n" + i2.city;
        }
    }
}
