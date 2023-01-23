using Microsoft.Win32;
using PuranLai.Algorithms;
using PuranLai.Tools;
using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static OverTop.CommonWindowOps;
using static System.Windows.Forms.DataFormats;

namespace OverTop.Floatings
{
    /// <summary>
    /// PropertyWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PropertyWindow : Window
    {
        bool saveSettings = true;
        Window CurrentWindow;
        public PropertyWindow(HangerWindow window)
        {
            InitializeComponent();
            CurrentWindow = window;
            WidthTextBox.Text = CurrentWindow.Width.ToString();
            HeightTextBox.Text = CurrentWindow.Height.ToString();
            System.Windows.Media.Color color = ((SolidColorBrush)CurrentWindow.Background).Color;
            ColorTextBox.Text = ColorTranslator.ToHtml(System.Drawing.Color.FromArgb(color.R, color.G, color.B));
            Title += " - " + CurrentWindow.Title;

            ButtonStackPanel.Visibility = Visibility.Visible;
            ToolTip = Title;

            AddTextContentImage.Source = MainWindow.GetIcon("Text");
            AddImageContentImage.Source = MainWindow.GetIcon("Image");
        }
        public PropertyWindow(RecentWindow window)
        {
            InitializeComponent();
            CurrentWindow = window;
            WidthTextBox.Text = CurrentWindow.Width.ToString();
            HeightTextBox.Text = CurrentWindow.Height.ToString();
            System.Windows.Media.Color color = ((SolidColorBrush)CurrentWindow.Background).Color;
            ColorTextBox.Text = ColorTranslator.ToHtml(System.Drawing.Color.FromArgb(color.R, color.G, color.B));
            Title += " - " + CurrentWindow.Title;

            ToolTip = Title;

            AddTextContentImage.Source = MainWindow.GetIcon("Text");
            AddImageContentImage.Source = MainWindow.GetIcon("Image");
        }

        public void TextPanel_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            StackPanel currentStackPanel = (StackPanel)sender;
            TextBlock currentTextBlock = (TextBlock)currentStackPanel.Children[0];
            if (Keyboard.IsKeyDown(Key.M))
            {
                TextWindow textWindow = new("请输入文本：", currentTextBlock.Text);
                textWindow.ShowDialog();
                currentTextBlock.Text = textWindow.result;
            }
            else if (Keyboard.IsKeyDown(Key.R))
            {
                ((StackPanel)currentStackPanel.Parent).Children.Remove(currentStackPanel);
            }
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
            ParsingResult result = Parse.ParseFromString(WidthTextBox.Text, 800);
            if (result.message != "")
            {
                MessageBox.Show(result.message);
                WidthTextBox.Text = CurrentWindow.Width.ToString();
                e.Cancel = true;
            }
            CurrentWindow.Width = Parse.ParseFromString(WidthTextBox.Text, 1200).number;

            result = Parse.ParseFromString(HeightTextBox.Text, 800);
            if (result.message != "")
            {
                MessageBox.Show(result.message);
                HeightTextBox.Text = CurrentWindow.Height.ToString();
                e.Cancel = true;
            }
            CurrentWindow.Height = Parse.ParseFromString(HeightTextBox.Text, 1200).number;

            System.Drawing.Color color = ColorTranslator.FromHtml(ColorTextBox.Text);
            var mediaColor = System.Windows.Media.Color.FromRgb(color.R, color.G, color.B);
            CurrentWindow.Background = new SolidColorBrush(mediaColor);
        }

        private void DefaultButton_Click(object sender, RoutedEventArgs e)
        {
            WindowProperty hangerProperty = new(), recentProperty = new();
            if (CurrentWindow is HangerWindow)
            {
                ParsingResult union = Parse.ParseFromString(WidthTextBox.Text, 800);
                if (union.message != "")
                {
                    MessageBox.Show(union.message);
                    WidthTextBox.Text = CurrentWindow.Width.ToString();
                    return;
                }
                hangerProperty.width = Parse.ParseFromString(WidthTextBox.Text, 1200).number;

                union = Parse.ParseFromString(HeightTextBox.Text, 800);
                if (union.message != "")
                {
                    MessageBox.Show(union.message);
                    HeightTextBox.Text = CurrentWindow.Height.ToString();
                    return;
                }
                hangerProperty.height = Parse.ParseFromString(HeightTextBox.Text, 1200).number;

                System.Drawing.Color color = ColorTranslator.FromHtml(ColorTextBox.Text);
                hangerProperty.backGroundColor = System.Windows.Media.Color.FromRgb(color.R, color.G, color.B);
                App.AppSettings.HangerWindowSettings = hangerProperty;
            }
            else if (CurrentWindow is RecentWindow)
            {
                ParsingResult union = Parse.ParseFromString(WidthTextBox.Text, 800);
                if (union.message != "")
                {
                    MessageBox.Show(union.message);
                    WidthTextBox.Text = CurrentWindow.Width.ToString();
                    return;
                }
                recentProperty.width = Parse.ParseFromString(WidthTextBox.Text, 1200).number;

                union = Parse.ParseFromString(HeightTextBox.Text, 800);
                if (union.message != "")
                {
                    MessageBox.Show(union.message);
                    HeightTextBox.Text = CurrentWindow.Height.ToString();
                    return;
                }
                recentProperty.height = Parse.ParseFromString(HeightTextBox.Text, 1200).number;

                System.Drawing.Color color = ColorTranslator.FromHtml(ColorTextBox.Text);
                recentProperty.backGroundColor = System.Windows.Media.Color.FromRgb(color.R, color.G, color.B);
                App.AppSettings.RecentWindowSettings = recentProperty;
            }

            App.AppSettings.Save();
        }

        private void AddTextButton_Click(object sender, RoutedEventArgs e)
        {
            TextWindow textWindow = new("请输入文本：", null);
            textWindow.ShowDialog();
            string? text = textWindow.result;

            if (string.IsNullOrEmpty(text))
                return;

            TextBlock newTextBlock = new()
            {
                TextWrapping = TextWrapping.Wrap,
                Style = (Style)FindResource("ContentTextBlockStyle"),
                Text = text
            };

            StackPanel textPanel = new();
            textPanel.MouseLeftButtonDown += TextPanel_MouseLeftButtonDown;
            textPanel.Children.Add(newTextBlock);
            ((HangerWindow)CurrentWindow).ContentPanel.Children.Add(textPanel);

            Close();
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
                    System.Windows.Controls.Image newImage = new();

                    StackPanel imagePanel = new();
                    imagePanel.Children.Add(newImage);
                    imagePanel.MouseLeftButtonDown += ImagePanel_MouseLeftButtonDown;
                    ((HangerWindow)CurrentWindow).ContentPanel.Children.Add(imagePanel);
                }
                catch { }
            }
            Close();
        }

        public static void ImagePanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.R))
            {
                ((StackPanel)((StackPanel)sender).Parent).Children.Remove((StackPanel)sender);
            }
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
