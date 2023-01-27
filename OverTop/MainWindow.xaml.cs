using PuranLai.Algorithms;
using System;
using System.Drawing;
using System.Resources;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace OverTop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool MenuClosed = true;

        Uri PropertyUri = new Uri("/Pages/StaticPropertyPage.xaml", UriKind.Relative);
        Uri FloatingUri = new Uri("/Pages/FloatingPanelPage.xaml", UriKind.Relative);
        public MainWindow()
        {
            InitializeComponent();
            Task.Run(() => Dispatcher.BeginInvoke(() => { LoadIcons(); }));
            App.MainWindow = this;

            FloatingListBoxItem.IsSelected = true;
            TitleTextBlock.Text = "浮窗控制";
            ContentFrame.NavigationService.Navigate(FloatingUri);
        }

        private void LoadIcons()
        {
            Icon = GetIcon("icon");
        }

        public static ImageSource GetIcon(string name)
        {
            ResourceManager Loader = OverTop.Resources.ResourcesFile.ResourceManager;
            object? resObj = Loader.GetObject(name);
            if (resObj is not null)
            {
                Bitmap icon = new((System.Drawing.Image)resObj);
                return Imaging.CreateBitmapSourceFromHBitmap(icon.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            return Imaging.CreateBitmapSourceFromHBitmap(0, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }

        private void NavigationButton_Click(object sender, RoutedEventArgs e)
        {
            if (MenuClosed)
            {
                Task.Run(() => Dispatcher.BeginInvoke(new Action(() => { MenuOpen(); })));
            }
            else
            {
                Task.Run(() => Dispatcher.BeginInvoke(new Action(() => { MenuClose(); })));
            }
            MenuClosed = !MenuClosed;
        }

        private unsafe void MenuOpen()
        {
            Action<double> SetPanelWidth = new((double value) =>
            {
                double width = value;
                Action<double> set = new((double value) => { MenuStackPanel.Width = value; });
                Dispatcher.Invoke(set, width);
            });
            fixed (bool* isOpened = &MenuClosed)
            {
                Animation open = new Animation(200, MenuStackPanel.Width, 160, Animation.GetSineValue, SetPanelWidth, 50, Flag: isOpened);
                open.StartAnimationAsync();
            }
        }

        private unsafe void MenuClose()
        {
            Action<double> SetPanelWidth = new((double value) =>
            {
                double width = value;
                Action<double> set = new((double value) => { MenuStackPanel.Width = value; });
                Dispatcher.Invoke(set, width);
            });
            fixed (bool* isOpened = &MenuClosed)
            {
                Animation open = new Animation(200, MenuStackPanel.Width, 45, Animation.GetSineValue, SetPanelWidth, 50, Flag: isOpened);
                open.StartAnimationAsync();
            }
        }

        private void ContentListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ContentListBox.SelectedItem == PropertyListBoxItem)
            {
                ContentFrame.NavigationService.Navigate(PropertyUri);
            }
            else if (ContentListBox.SelectedItem == FloatingListBoxItem)
            {
                ContentFrame.NavigationService.Navigate(FloatingUri);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ContentFrame.CanGoBack)
            {
                return;
            }
            ContentFrame.GoBack();
        }

        private void ContentFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (ContentFrame.Source == PropertyUri)
            {
                TitleTextBlock.Text = "系统属性";
                PropertyListBoxItem.IsSelected = true;
            }
            else if (ContentFrame.Source == FloatingUri)
            {
                TitleTextBlock.Text = "浮窗控制";
                FloatingListBoxItem.IsSelected = true;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                Disappear();
            }
        }

        private void DockPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private async void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            Disappear();
            await Task.Delay(500);
            this.WindowState = WindowState.Minimized;
        }

        private async void DisappearButton_Click(object sender, RoutedEventArgs e)
        {
            Disappear();
            await Task.Delay(500);
            Visibility = Visibility.Collapsed;
        }

        private void CloseWindowButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("是否要退出 Over Top ？", "退出 Over Top", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.No)
            {
                return;
            }
            else
            {
                App.Current.Shutdown();
            }
        }

        private void Disappear()
        {
            Action<double> SetOpacity = new((double value) =>
            {
                double opacity = value;
                Action<double> set = new((double value) =>
                {
                    Opacity = opacity;
                });
                Dispatcher.Invoke(set, opacity);
            });
            unsafe
            {
                Animation animation = new(500, Opacity, 0, Animation.GetLinearValue, SetOpacity);
                animation.StartAnimationAsync();
            }
        }

        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Opacity = 1;
        }
    }
}
