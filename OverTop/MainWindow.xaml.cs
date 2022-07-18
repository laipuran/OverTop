using System;
using System.Windows;
using System.Windows.Media.Animation;
using System.Resources;
using System.Drawing;
using static OverTop.App;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace OverTop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool MenuClosed = true;
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

        // Naviagtion Part
        private void PropertyListBoxItem_Selected(object sender, RoutedEventArgs e)
        {
            ContentFrame.NavigationService.Navigate(new Uri("/Pages/StaticPropertyPage.xaml", UriKind.Relative));
        }

        private void FloatingListBoxItem_Selected(object sender, RoutedEventArgs e)
        {
            ContentFrame.NavigationService.Navigate(new Uri("/Pages/FloatingPanelPage.xaml", UriKind.Relative));
        }
        
        private void ContentListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ContentListBox.SelectedItem == PropertyListBoxItem)
            {
                TitleTextBlock.Text = "系统静态属性";
            }
            else if (ContentListBox.SelectedItem == FloatingListBoxItem)
            {
                TitleTextBlock.Text = "浮窗控制面板";
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (Window window in Pages.FloatingPanelPage.windows)
            {
                window.Close();
            }
        }
    }
}
