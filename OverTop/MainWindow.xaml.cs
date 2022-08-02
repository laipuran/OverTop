using System;
using System.Drawing;
using System.Resources;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Animation;
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
            FloatingListBoxItem.IsSelected = true;
            TitleTextBlock.Text = "浮窗控制面板";
            ContentFrame.NavigationService.Navigate(new Uri("/Pages/FloatingPanelPage.xaml", UriKind.Relative));
            ResourceManager Loader = OverTop.Resources.ResourcesFile.ResourceManager;
#pragma warning disable CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
#pragma warning disable CS8604 // 引用类型参数可能为 null。
            Bitmap icon = new((Image)Loader.GetObject("icon"));
#pragma warning restore CS8604 // 引用类型参数可能为 null。
#pragma warning restore CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
            Icon = Imaging.CreateBitmapSourceFromHBitmap(icon.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            Pages.StaticPropertyPage.ColorChanged();
        }

        private void NavigationButton_Click(object sender, RoutedEventArgs e)
        {
            if (MenuClosed)
            {
                Storyboard openMenu = (Storyboard)FindResource("MenuOpen");
                openMenu.Begin();
            }
            else
            {
                Storyboard closeMenu = (Storyboard)FindResource("MenuClose");
                closeMenu.Begin();
            }
            MenuClosed = !MenuClosed;
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (Window window in Pages.FloatingPanelPage.windows)
            {
                window.Close();
            }
        }

        private void ContentFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (ContentFrame.Source == PropertyUri)
            {
                TitleTextBlock.Text = "系统静态属性";
                PropertyListBoxItem.IsSelected = true;
            }
            else if (ContentFrame.Source == FloatingUri)
            {
                TitleTextBlock.Text = "浮窗控制面板";
                PropertyListBoxItem.IsSelected = true;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (ContentFrame.CanGoBack)
            {
                ContentFrame.GoBack();
            }
        }
    }
}
