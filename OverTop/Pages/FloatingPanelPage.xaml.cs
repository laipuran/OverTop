using Microsoft.Win32;
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
            Window newHanger = new Floatings.HangerWindow();
            newHanger.Title = Guid.NewGuid().ToString();
            newHanger.ToolTip = "Hanger Window - " + hangers;
            newHanger.Width = App.settings.HangerWindowSettings.width;
            newHanger.Height = App.settings.HangerWindowSettings.height;
            newHanger.Background = new SolidColorBrush(App.settings.HangerWindowSettings.backGroundColor);
            newHanger.Opacity = App.settings.HangerWindowSettings.alpha == 0.0 ? 0.8 : App.settings.HangerWindowSettings.alpha;
            newHanger.Show();
            windows.Add(newHanger);
        }

        private void RecentWindowButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            recents++;
            Window newRecent = new Floatings.RecentWindow();
            newRecent.Title = Guid.NewGuid().ToString();
            newRecent.ToolTip = "Recent Window - " + recents;
            newRecent.Width = App.settings.RecentWindowSettings.width;
            newRecent.Height = App.settings.RecentWindowSettings.height;
            newRecent.Background = new SolidColorBrush(App.settings.RecentWindowSettings.backGroundColor);
            newRecent.Opacity = App.settings.RecentWindowSettings.alpha == 0.0 ? 0.8 : App.settings.RecentWindowSettings.alpha;
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

            HangerWindow newHanger = new Floatings.HangerWindow();
#pragma warning disable CS8602 // 解引用可能出现空引用。
            newHanger.Width = windowClass.width;
            newHanger.Height = windowClass.height;
            System.Drawing.Color tempColor = ColorTranslator.FromHtml(windowClass.backgroundColor);
            System.Windows.Media.Color color = System.Windows.Media.Color.FromRgb(tempColor.R, tempColor.G, tempColor.B);
            newHanger.Background = new SolidColorBrush(color);
            newHanger.Opacity = windowClass.alpha;
            newHanger.ToolTip = "Hanger Window - " + hangers;
            newHanger.Title = fileName;
#pragma warning restore CS8602 // 解引用可能出现空引用。

            StackPanel ContentStackPanel = (StackPanel)((ScrollViewer)newHanger.Content).Content;
            foreach (KeyValuePair<HangerWindowProperty.ContentType, string> pair in windowClass.contents)
            {
                if (pair.Key == HangerWindowProperty.ContentType.Text)
                {
                    TextBlock newTextBlock = new();
                    newTextBlock.Style = (Style)FindResource("ContentTextBlockStyle");
                    newTextBlock.Text = pair.Value;
                    StackPanel newStackPanel = new();
                    newStackPanel.Children.Add(newTextBlock);
                    newStackPanel.MouseLeftButtonDown += TextPanel_MouseLeftButtonDown;
                    ContentStackPanel.Children.Add(newStackPanel);
                }
                else if (pair.Key == HangerWindowProperty.ContentType.Image)
                {
                    try
                    {
                        System.Windows.Controls.Image newImage = new();
                        newImage.Source = new BitmapImage(new Uri(pair.Value));
                        StackPanel newStackPanel = new();
                        newStackPanel.Children.Add(newImage);
                        newStackPanel.MouseLeftButtonDown += NewStackPanel_MouseLeftButtonDown; ;
                        ContentStackPanel.Children.Add(newStackPanel);
                    }
                    catch
                    {
                        throw new PuranLai.CustomException("Image not exists!");
                    }
                }
            }
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
