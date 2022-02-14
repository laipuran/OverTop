using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace OverTop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    // TODO: Change Main Menu to List Box 
    public partial class MainWindow : Window
    {
        bool MenuClosed = true;
        public MainWindow()
        {
            InitializeComponent();
            ContentFrame.NavigationService.Navigate(new Uri("/Pages/StaticPropertyPage.xaml", UriKind.Relative));
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
        private void FloatingMenuButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.NavigationService.Navigate(new Uri("/Pages/FloatingPanelPage.xaml", UriKind.Relative));
        }
        private void StaticPropertyButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.NavigationService.Navigate(new Uri("/Pages/StaticPropertyPage.xaml", UriKind.Relative));
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (ContentFrame.NavigationService.CanGoBack)
            {
                ContentFrame.NavigationService.GoBack();
            }
        }
    }
}
