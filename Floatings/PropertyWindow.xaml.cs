using LuckDraw;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OverTop.Floatings
{
    /// <summary>
    /// PropertyWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PropertyWindow : Window
    {
        public PropertyWindow()
        {
            InitializeComponent();
            AlphaSlider.Value = App.currentWindow.Opacity;
            WidthTextBox.Text = App.currentWindow.Width.ToString();
            HeightTextBox.Text = App.currentWindow.Height.ToString();
            System.Windows.Media.Color color = ((SolidColorBrush)App.currentWindow.Background).Color;
            ColorTextBox.Text = ColorTranslator.ToHtml(System.Drawing.Color.FromArgb(color.R, color.G, color.B));
        }

        private void AlphaSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            App.parameterClass.alpha = Math.Round(AlphaSlider.Value, 2);
            AlphaTextBlock.Text = App.parameterClass.alpha.ToString();
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

            System.Drawing.Color color = ColorTranslator.FromHtml(ColorTextBox.Text);
            Settings.Default.colorR = color.R;
            Settings.Default.colorG = color.G;
            Settings.Default.colorB = color.B;
            Settings.Default.Save();
        }
    }
}
