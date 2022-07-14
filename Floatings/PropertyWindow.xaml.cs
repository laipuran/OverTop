using LuckDraw;
using Microsoft.Win32;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace OverTop.Floatings
{
    /// <summary>
    /// PropertyWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PropertyWindow : Window
    {
        Thickness margin = new();
        Window textWindow = new();
        StackPanel newTextPanel = new();
        ScrollViewer scrollViewer = new();
        TextBox newTextBox = new();
        Button OKButton = new();
        public PropertyWindow()
        {
            InitializeComponent();
            AlphaSlider.Value = App.currentWindow.Opacity;
            WidthTextBox.Text = App.currentWindow.Width.ToString();
            HeightTextBox.Text = App.currentWindow.Height.ToString();
            System.Windows.Media.Color color = ((SolidColorBrush)App.currentWindow.Background).Color;
            ColorTextBox.Text = ColorTranslator.ToHtml(System.Drawing.Color.FromArgb(color.R, color.G, color.B));

            if (App.windowType == App.WindowType.Hanger)
            {
                InitializeTextWindow();
                Title += " - Hanger Window";
            }
            else
                Title += " - Recent Window";
            ToolTip = Title;
        }
        private void InitializeTextWindow()
        {
            ButtonStackPanel.Visibility = Visibility.Visible;

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

            newTextPanel.Children.Add(newTextBox);
            newTextPanel.Children.Add(OKButton);

            scrollViewer.Content = newTextPanel;
            scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;

            textWindow.Content = scrollViewer;
            textWindow.LostFocus += OKButton_Click;
        }
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (newTextBox.Text != string.Empty)
            {
                TextBlock newTextBlock = new()
                {
                    TextWrapping = TextWrapping.Wrap,
                    Style = (Style)FindResource("ContentTextBlockStyle"),
                    Text = newTextBox.Text
                };

                newTextPanel.Visibility = Visibility.Collapsed;
                App.contentStackPanel.Children.Add(newTextBlock);
                newTextBox.Text = "";

                textWindow.Close();
            }
        }

        private void AlphaSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            AlphaTextBlock.Text = Math.Round(AlphaSlider.Value, 2).ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            App.currentWindow.Close();
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
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
            Settings.Default.width = Algorithm.Parser(WidthTextBox.Text, 1200).number;

            union = Algorithm.Parser(HeightTextBox.Text, 800);
            if (union.message != "")
            {
                MessageBox.Show(union.message);
                HeightTextBox.Text = App.currentWindow.Height.ToString();
                return;
            }
            Settings.Default.height = Algorithm.Parser(HeightTextBox.Text, 1200).number;
            Settings.Default.alpha = AlphaSlider.Value;
            Settings.Default.color = ColorTranslator.FromHtml(ColorTextBox.Text);

            Settings.Default.Save();
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
            if (openFileDialog.ShowDialog() != DialogResult)
            {
                try
                {
                    System.Windows.Controls.Image newImage = new()
                    {
                        Source = new BitmapImage(new Uri(openFileDialog.FileName))
                    };
                    App.contentStackPanel.Children.Add(newImage);
                }
                catch (UriFormatException)
                {
                    return;
                }
            }
            Close();
        }
    }
}
