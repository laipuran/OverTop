using System;
using System.Windows;
using System.Windows.Media.Animation;
using static OverTop.App;

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
