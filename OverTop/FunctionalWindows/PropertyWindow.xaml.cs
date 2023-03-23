using Microsoft.Win32;
using OverTop.ContentWindows;
using PuranLai.Algorithms;
using PuranLai.Tools;
using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace OverTop.FunctionalWindows
{
    /// <summary>
    /// PropertyWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PropertyWindow : Window
    {
        bool saveSettings = true;
        Window CurrentWindow;
        public PropertyWindow(CustomWindow window)
        {
            InitializeComponent();
            ButtonStackPanel.Visibility = Visibility.Visible;
            CurrentWindow = window;
            CommonSettings(window);
        }

        public PropertyWindow(RecentWindow window)
        {
            InitializeComponent();
            CurrentWindow = window;
            CommonSettings(window);
        }

        private void CommonSettings(Window window)
        {
            WidthTextBox.Text = CurrentWindow.Width.ToString();
            HeightTextBox.Text = CurrentWindow.Height.ToString();
            System.Windows.Media.Color color = ((SolidColorBrush)CurrentWindow.Background).Color;
            ColorTextBox.Text = ColorTranslator.ToHtml(System.Drawing.Color.FromArgb(color.R, color.G, color.B));
            Title += " - " + CurrentWindow.Title;

            ToolTip = Title;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CurrentWindow.Close();
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!saveSettings)
            {
                return;
            }
            WindowProperty? property = GetWindowProperty();

            if (property == null)
                return;

            if (CurrentWindow is CustomWindow)
            {
                ((CustomWindow)CurrentWindow).Property.FromProperty(property);
                ((CustomWindow)CurrentWindow).Reload();
            }
            else if (CurrentWindow is RecentWindow)
            {
                ((RecentWindow)CurrentWindow).Property.FromProperty(property);
                ((RecentWindow)CurrentWindow).Reload();
            }
        }

        private void DefaultButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentWindow is CustomWindow)
            {
                WindowProperty? hangerProperty = GetWindowProperty();
                if (hangerProperty is null)
                    return;

                App.AppSettings.HangerWindowSettings = hangerProperty;
            }
            else if (CurrentWindow is RecentWindow)
            {
                WindowProperty? recentProperty = GetWindowProperty();
                if (recentProperty is null)
                    return;

                App.AppSettings.RecentWindowSettings = recentProperty;
            }

            App.AppSettings.Save();
        }

        private WindowProperty? GetWindowProperty()
        {
            WindowProperty hangerProperty = new();
            ParsingResult union = Parse.ParseFromString(WidthTextBox.Text, 800);
            if (union.message != "")
            {
                MessageBox.Show(union.message);
                WidthTextBox.Text = CurrentWindow.Width.ToString();
                return null;
            }
            hangerProperty.Width = union.number;

            union = Parse.ParseFromString(HeightTextBox.Text, 800);
            if (union.message != "")
            {
                MessageBox.Show(union.message);
                HeightTextBox.Text = CurrentWindow.Height.ToString();
                return null;
            }
            hangerProperty.Height = union.number;

            System.Drawing.Color color = ColorTranslator.FromHtml(ColorTextBox.Text);
            hangerProperty.BackgroundColor = System.Windows.Media.Color.FromRgb(color.R, color.G, color.B);
            return hangerProperty;
        }

        private void AddTextButton_Click(object sender, RoutedEventArgs e)
        {
            TextWindow textWindow = new("请输入文本：", null);
            textWindow.ShowDialog();
            string? text = textWindow.result;

            if (string.IsNullOrEmpty(text))
                return;

            ((CustomWindow)CurrentWindow).Property.Contents.Add(new(HangerWindowProperty.ContentType.Text, text));

            ((CustomWindow)CurrentWindow).Reload();
        }

        private void AddPictureButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                Multiselect = false,
                RestoreDirectory = true,
                Filter = "图片|*.jpeg; *.jpg; *.png|所有文件|*.*",
                FilterIndex = 1
            };
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    MemoryStream ms = new();
                    Bitmap bitmap = new Bitmap(openFileDialog.FileName);
                    bitmap.Save(ms, bitmap.RawFormat);
                    byte[] bytes = ms.GetBuffer();
                    string base64 = Convert.ToBase64String(bytes);
                    ms.Close();

                    ((CustomWindow)CurrentWindow).Property.Contents.Add(new(HangerWindowProperty.ContentType.Image, base64));
                }
                catch { }
            }
            ((CustomWindow)CurrentWindow).Reload();
        }

        private void AddLinkButton_Click(object sender, RoutedEventArgs e)
        {
            TextWindow textWindow = new("请输入文本：", null);
            textWindow.ShowDialog();
            string? text = textWindow.result;

            if (string.IsNullOrEmpty(text))
                return;

            ((CustomWindow)CurrentWindow).Property.Link = text;
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                saveSettings = false;
                Close();
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

    }
}
