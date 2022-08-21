using Microsoft.Win32;
using Newtonsoft.Json;
using OverTop.Floatings;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
        private void RecentWindowButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            recents++;
            Window newRecent = new Floatings.RecentWindow();
            newRecent.Title = Guid.NewGuid().ToString();
            newRecent.ToolTip = "Recent Window - " + recents;
            newRecent.Width = App.recentSettingsClass.width;
            newRecent.Height = App.recentSettingsClass.height;
            newRecent.Background = new SolidColorBrush(App.recentSettingsClass.backGroundColor);
            newRecent.Opacity = App.recentSettingsClass.alpha == 0.0 ? 0.8 : App.recentSettingsClass.alpha;
            newRecent.Show();
            windows.Add(newRecent);
        }

        private void HangerWindowButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            hangers++;
            Window newHanger = new Floatings.HangerWindow();
            newHanger.Title = Guid.NewGuid().ToString();
            newHanger.Width = App.hangerSettingsClass.width;
            newHanger.Height = App.hangerSettingsClass.height;
            newHanger.Background = new SolidColorBrush(App.hangerSettingsClass.backGroundColor);
            newHanger.Opacity = App.hangerSettingsClass.alpha == 0.0 ? 0.8 : App.hangerSettingsClass.alpha;
            newHanger.Show();
            windows.Add(newHanger);

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
#pragma warning disable CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
            HangerWindowClass windowClass = JsonConvert.DeserializeObject<HangerWindowClass>(json);
#pragma warning restore CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。

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
            foreach (KeyValuePair<HangerWindowClass.ContentType, string> pair in windowClass.contents)
            {
                if (pair.Key == HangerWindowClass.ContentType.Text)
                {
                    TextBlock newTextBlock = new();
                    newTextBlock.Style = (Style)FindResource("ContentTextBlockStyle");
                    newTextBlock.Text = pair.Value;
                    StackPanel newStackPanel = new();
                    newStackPanel.Children.Add(newTextBlock);
                    newStackPanel.MouseLeftButtonDown += TextPanel_MouseLeftButtonDown;
                    ContentStackPanel.Children.Add(newStackPanel);
                }
                else if (pair.Key == HangerWindowClass.ContentType.Image)
                {
                    System.Windows.Controls.Image newImage = new();
                    newImage.Source = new BitmapImage(new Uri(pair.Value));
                    StackPanel newStackPanel = new();
                    newStackPanel.Children.Add(newImage);
                    newStackPanel.MouseLeftButtonDown += NewStackPanel_MouseLeftButtonDown; ;
                    ContentStackPanel.Children.Add(newStackPanel);
                }
            }
            windows.Add(newHanger);
            newHanger.Show();
        }

        private void NewStackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.R))
            {
                ((StackPanel)((StackPanel)sender).Parent).Children.Remove((StackPanel)sender);
            }
        }

        private void InitializeTextWindow(string text, Window textWindow, RoutedEventHandler routedEventHandler)
        {
            Thickness margin = new();
            StackPanel newTextPanel = new();
            ScrollViewer scrollViewer = new();
            TextBox newTextBox = new();
            Button OKButton = new();

            OKButton.Style = (Style)FindResource("ContentButtonStyle");
            OKButton.Content = "OK";
            margin.Left = 20;
            margin.Right = 20;
            margin.Top = 10;
            OKButton.Margin = margin;
            OKButton.Click += OKButton_Click;

            textWindow.Width = 150;
            textWindow.Height = 150;
            textWindow.Topmost = true;

            newTextBox.Style = (Style)FindResource("ContentTextBoxStyle");
            newTextBox.TextWrapping = TextWrapping.Wrap;
            newTextBox.Width = 300 - 5 - OKButton.Width;
            newTextBox.Margin = margin;
            newTextBox.Text = text;

            newTextPanel.Children.Add(newTextBox);
            newTextPanel.Children.Add(OKButton);

            scrollViewer.Content = newTextPanel;
            scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;

            textWindow.Content = scrollViewer;
            textWindow.Loaded += TextWindow_Loaded;
            textWindow.LostFocus += routedEventHandler;
        }

        private void TextWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ((StackPanel)((ScrollViewer)((Window)sender).Content).Content).Children[0].Focus();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender.GetType() != typeof(Window))
            {
                return;
            }
            TextBox newTextBox = (TextBox)((StackPanel)((ScrollViewer)((Window)sender).Content).Content).Children[0];
            if (newTextBox.Text != string.Empty)
            {
                TextBlock newTextBlock = new()
                {
                    TextWrapping = TextWrapping.Wrap,
                    Style = (Style)FindResource("ContentTextBlockStyle"),
                    Text = newTextBox.Text
                };
                StackPanel textPanel = new();
                textPanel.MouseLeftButtonDown += TextPanel_MouseLeftButtonDown;
                textPanel.Children.Add(newTextBlock);
                App.contentStackPanel.Children.Add(textPanel);
                newTextBox.Text = "";
                ((Window)sender).Close();
            }
        }
        private void OKButton_Click_Modify(object sender, RoutedEventArgs e)
        {
            if (sender.GetType() != typeof(Window))
            {
                return;
            }
            TextBox newTextBox = (TextBox)((StackPanel)((ScrollViewer)((Window)sender).Content).Content).Children[0];
            if (newTextBox.Text != string.Empty)
            {
                TextBlock newTextBlock = new()
                {
                    TextWrapping = TextWrapping.Wrap,
                    Style = (Style)FindResource("ContentTextBlockStyle"),
                    Text = newTextBox.Text
                };
                StackPanel textPanel = new();
                App.contentStackPanel.Children.Add(newTextBlock);
                newTextBox.Text = "";
            }
            ((Window)sender).Close();
        }

        public void TextPanel_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.M))
            {
                Window textWindowModify = new();
                InitializeTextWindow(((TextBlock)((StackPanel)sender).Children[0]).Text, textWindowModify, OKButton_Click_Modify);
                ((StackPanel)sender).Children.Clear();
                App.contentStackPanel = (StackPanel)sender;
                textWindowModify.ShowDialog();
            }
            else if (Keyboard.IsKeyDown(Key.R))
            {
                ((StackPanel)((StackPanel)sender).Parent).Children.Remove((StackPanel)sender);
            }
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

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (Window window in windows)
            {
                window.Close();
            }
        }

    }
}
