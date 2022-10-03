﻿using Microsoft.Win32;
using Newtonsoft.Json;
using OverTop.Floatings;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static OverTop.API;

namespace OverTop.Pages
{
    /// <summary>
    /// FloatingPanelPage.xaml 的交互逻辑
    /// </summary>
    public partial class FloatingPanelPage : Page
    {
        public static List<Window> windows = new();
        public static int recents = 0, hangers = 0;

        public FloatingPanelPage()
        {
            InitializeComponent();
            HangerToolTipImage.Source = MainWindow.GetIcon("HangerWindow");
            RecentToolTipImage.Source = MainWindow.GetIcon("RecentWindow");
        }

        private void HangerWindowButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            hangers++;
            Window newHanger = new Floatings.HangerWindow(HangerWindowProperty.GetDefaultProperty());
            newHanger.Show();
            windows.Add(newHanger);
        }

        private void RecentWindowButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            recents++;
            Window newRecent = new Floatings.RecentWindow(RecentWindowProperty.GetDefaultProperty());
            newRecent.Show();
            windows.Add(newRecent);
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
            hangers++;
            HangerWindowProperty windowClass = new();
            try
            {
#pragma warning disable CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
                windowClass = JsonConvert.DeserializeObject<HangerWindowProperty>(json);
#pragma warning restore CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
            }
            catch
            {
                throw new PuranLai.CustomException("Not the right JSON structure!");
            }

#pragma warning disable CS8602 // 解引用可能出现空引用。
            windowClass.guid = fileName;
#pragma warning restore CS8602 // 解引用可能出现空引用。
            HangerWindow newHanger = new Floatings.HangerWindow(windowClass);

            windows.Add(newHanger);
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
            App.weatherWindow.Show();
            windows.Add(App.weatherWindow);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (Window window in windows)
            {
                window.Close();
            }
        }
    }
}
