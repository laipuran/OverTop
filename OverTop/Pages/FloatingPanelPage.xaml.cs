using Microsoft.Win32;
using Newtonsoft.Json;
using OverTop.Floatings;
using PuranLai.Algorithms;
using PuranLai.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OverTop.Pages
{
    /// <summary>
    /// FloatingPanelPage.xaml 的交互逻辑
    /// </summary>
    public partial class FloatingPanelPage : Page
    {
        public FloatingPanelPage()
        {
            InitializeComponent();

            if (App.AppSettings.ip is null)
            {
                WeatherStackPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void HangerWindowButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Window newHanger = new Floatings.HangerWindow(HangerWindowProperty.GetDefaultProperty());
            newHanger.Show();
        }

        private void RecentWindowButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Window newRecent = new Floatings.RecentWindow(RecentWindowProperty.GetDefaultProperty());
            newRecent.Show();
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\OverTop\\HangerWindows\\";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            OpenFileDialog openFileDialog = new()
            {
                Multiselect = true,
                InitialDirectory = path,
                Filter = "JSON 文件|*.json",
                FilterIndex = 1
            };
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filePath in openFileDialog.FileNames)
                {
                    GetWindowFromString(File.ReadAllText(filePath), Path.GetFileNameWithoutExtension(filePath));
                }
            }
        }

        private void GetWindowFromString(string json, string fileName)
        {
            HangerWindowProperty windowClass = new();
            try
            {
#pragma warning disable CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
                windowClass = JsonConvert.DeserializeObject<HangerWindowProperty>(json);
#pragma warning restore CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
            }
            catch
            {
                throw new CustomException("Not the right JSON structure!");
            }

#pragma warning disable CS8602 // 解引用可能出现空引用。
            windowClass.guid = fileName;
#pragma warning restore CS8602 // 解引用可能出现空引用。
            HangerWindow newHanger = new Floatings.HangerWindow(windowClass);

            newHanger.Show();
        }

        private void Page_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string filePath in files)
                {
                    if (filePath.EndsWith(".json"))
                    {
                        GetWindowFromString(File.ReadAllText(filePath), Path.GetFileNameWithoutExtension(filePath));
                    }
                }
            }
        }

        private void WeatherWindowButton_Click(object sender, RoutedEventArgs e)
        {
            App.WeatherWindow.Show();
        }

        private void ImportButton_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TextWindow textWindow = new("请输入网址：", null);
            textWindow.ShowDialog();
            if (string.IsNullOrEmpty(textWindow.result))
                return;
            WindowLoader.OpenFromInternet(textWindow.result);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            List<Window> copy = new();
            copy.AddRange(App.FloatingWindows);
            try
            {
                foreach (Window window in copy)
                {
                    window.Close();
                }

            }
            catch { }
        }
    }
}
