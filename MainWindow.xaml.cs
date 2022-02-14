using System;
using System.Windows;
using System.Windows.Media.Animation;
using static OverTop.App;

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
            PropertyListBoxItem.IsSelected = true;
            TitleTextBlock.Text = "Static Property";
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
                TitleTextBlock.Text = "Static Property";
            }
            else if (ContentListBox.SelectedItem == FloatingListBoxItem)
            {
                TitleTextBlock.Text = "Floating Control Panel";
            }
        }
    }
}
