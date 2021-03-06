using LuckDraw;
using Microsoft.Win32;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace OverTop.Floatings
{
    /// <summary>
    /// PropertyWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PropertyWindow : Window
    {
        Window textWindow = new();
        bool saveSettings = true;
        public PropertyWindow()
        {
            InitializeComponent();
            AlphaSlider.Value = App.currentWindow.Opacity;
            WidthTextBox.Text = App.currentWindow.Width.ToString();
            HeightTextBox.Text = App.currentWindow.Height.ToString();
            System.Windows.Media.Color color = ((SolidColorBrush)App.currentWindow.Background).Color;
            ColorTextBox.Text = ColorTranslator.ToHtml(System.Drawing.Color.FromArgb(color.R, color.G, color.B));
            Title += " - " + App.currentWindow.Title;

            if (App.windowType == App.WindowType.Hanger)
            {
                InitializeTextWindow("", textWindow, OKButton_Click);
            }
            else
            {
            }

            ToolTip = Title;
        }
        private void InitializeTextWindow(string text, Window textWindow, RoutedEventHandler routedEventHandler)
        {
            ButtonStackPanel.Visibility = Visibility.Visible;

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
            ((StackPanel)((ScrollViewer)textWindow.Content).Content).Children[0].Focus();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
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
            }
            ((Window)sender).Close();
        }
        private void OKButton_Click_Modify(object sender, RoutedEventArgs e)
        {
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

        private void AlphaSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (AlphaSlider.Value == 0.0)
            {
                AlphaSlider.Value = 0.8;
                return;
            }
            AlphaTextBlock.Text = Math.Round(AlphaSlider.Value, 2).ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            App.currentWindow.Close();
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!saveSettings)
            {
                return;
            }
            Union union = Algorithm.Parser(WidthTextBox.Text, 800);
            if (union.message != "")
            {
                MessageBox.Show(union.message);
                WidthTextBox.Text = App.currentWindow.Width.ToString();
                return;
            }
            App.parameterClass.width = Algorithm.Parser(WidthTextBox.Text, 1200).number;

            union = Algorithm.Parser(HeightTextBox.Text, 800);
            if (union.message != "")
            {
                MessageBox.Show(union.message);
                HeightTextBox.Text = App.currentWindow.Height.ToString();
                return;
            }
            App.parameterClass.height = Algorithm.Parser(HeightTextBox.Text, 1200).number;

            App.parameterClass.alpha = AlphaSlider.Value;
            System.Drawing.Color color = ColorTranslator.FromHtml(ColorTextBox.Text);
            App.parameterClass.backGroundColor = System.Windows.Media.Color.FromRgb(color.R, color.G, color.B);
        }

        private void DefaultButton_Click(object sender, RoutedEventArgs e)
        {
            Union union = Algorithm.Parser(WidthTextBox.Text, 800);
            if (union.message != "")
            {
                MessageBox.Show(union.message);
                WidthTextBox.Text = App.currentWindow.Width.ToString();
                return;
            }

            union = Algorithm.Parser(HeightTextBox.Text, 800);
            if (union.message != "")
            {
                MessageBox.Show(union.message);
                HeightTextBox.Text = App.currentWindow.Height.ToString();
                return;
            }
            if (App.windowType == App.WindowType.Hanger)
            {
                HangerSettings.Default.width = Algorithm.Parser(WidthTextBox.Text, 1200).number;
                HangerSettings.Default.height = Algorithm.Parser(HeightTextBox.Text, 1200).number;
                HangerSettings.Default.alpha = AlphaSlider.Value;
                HangerSettings.Default.color = ColorTranslator.FromHtml(ColorTextBox.Text);
                HangerSettings.Default.Save();
            }
            else
            {
                RecentSettings.Default.width = Algorithm.Parser(WidthTextBox.Text, 1200).number;
                RecentSettings.Default.height = Algorithm.Parser(HeightTextBox.Text, 1200).number;
                RecentSettings.Default.alpha = AlphaSlider.Value;
                RecentSettings.Default.color = ColorTranslator.FromHtml(ColorTextBox.Text);
                RecentSettings.Default.Save();
            }

        }

        private void AddTextButton_Click(object sender, RoutedEventArgs e)
        {
            textWindow.ShowDialog();
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
                    System.Windows.Controls.Image newImage = new()
                    {
                        Source = new BitmapImage(new Uri(openFileDialog.FileName))
                    };
                    StackPanel imagePanel = new();
                    imagePanel.Children.Add(newImage);
                    imagePanel.MouseLeftButtonDown += ImagePanel_MouseLeftButtonDown;
                    App.contentStackPanel.Children.Add(imagePanel);
                }
                catch (UriFormatException)
                {
                    return;
                }
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

    }
}
