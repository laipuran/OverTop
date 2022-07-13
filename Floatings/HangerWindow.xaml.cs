using Microsoft.Win32;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
namespace OverTop.Floatings
{
    /// <summary>
    /// HangerWindow.xaml 的交互逻辑
    /// OKButton 是编程加入的
    /// AddTextButton 是 HangerWindow.xaml 中 ContextMenu 的按钮
    /// </summary>
    public partial class HangerWindow : Window
    {
        StackPanel newTextPanel = new();
        TextBox newTextBox = new();
        Button OKButton = new();
        Thickness thickness = new();
        public HangerWindow()
        {
            InitializeComponent();
            Width = Settings.Default.width;
            Height = Settings.Default.height;
            Opacity = Settings.Default.alpha;
            App.parameterClass.backGroundColor = System.Windows.Media.Color.FromRgb(Settings.Default.colorR, Settings.Default.colorG, Settings.Default.colorB);
            Background = new SolidColorBrush(App.parameterClass.backGroundColor);
            // Add Text
            OKButton.Style = (Style)FindResource("ContentButtonStyle");
            OKButton.Content = "OK";
            thickness.Left = 5;
            OKButton.Margin = thickness;

            newTextBox.Style = (Style)FindResource("ContentTextBoxStyle");
            newTextBox.TextWrapping = TextWrapping.Wrap;
            newTextBox.Width = 180 - 5 - OKButton.Width;

            newTextPanel.Visibility = Visibility.Collapsed;
            newTextPanel.Orientation = Orientation.Horizontal;
            newTextPanel.Children.Add(newTextBox);
            newTextPanel.Children.Add(OKButton);
            ContentStackPanel.Children.Add(newTextPanel);

            newTextBox.LostKeyboardFocus += NewButton_Click;
            OKButton.Click += NewButton_Click;

        }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (System.Windows.Input.Mouse.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void AddTextButton_Click(object sender, RoutedEventArgs e)
        {
            newTextPanel.Visibility = Visibility.Visible;
            ContextMenu.IsOpen = false;
            newTextBox.Focus();
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            if (newTextBox.Text != string.Empty)
            {
                TextBlock newTextBlock = new()
                {
                    TextWrapping = TextWrapping.Wrap,
                    Width = 160,
                    Style = (Style)FindResource("ContentTextBlockStyle"),
                    Text = newTextBox.Text
                };

                newTextPanel.Visibility = Visibility.Collapsed;
                ContentStackPanel.Children.Add(newTextBlock);
                newTextBox.Text = "";
            }
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
                        Source = new BitmapImage(new System.Uri(openFileDialog.FileName))
                    };
                    ContentStackPanel.Children.Add(newImage);
                }
                catch (System.UriFormatException)
                {
                    return;
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            ContextMenu.IsOpen = false;
            Close();
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                Close();
            }
        }

        private void ContentStackPanel_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Scroller.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
        }

        private void Window_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Scroller.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
        }

        private void Window_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            App.currentWindow = this;
            Window propertyWindow = new PropertyWindow
            {
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            propertyWindow.ShowDialog();
            Opacity = App.parameterClass.alpha;
            Width = App.parameterClass.width;
            Height = App.parameterClass.height;
            Background = new SolidColorBrush(App.parameterClass.backGroundColor);
        }
    }
}
