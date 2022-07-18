using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using static OverTop.App;

namespace OverTop.Pages
{
    /// <summary>
    /// StaticPropertyPage.xaml 的交互逻辑
    /// </summary>
    public partial class StaticPropertyPage : Page
    {
        public StaticPropertyPage()
        {
            InitializeComponent();
            SystemEvents.UserPreferenceChanged += UserPreferenceChanged;
        }
        public void UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            ColorChanged();
        }
        private void ColorChanged()
        {
            SystemGlassBrushButton.Foreground = SystemParameters.WindowGlassBrush;
            SystemGlassBrushButton.Content = SystemParameters.WindowGlassBrush.ToString();
            DesktopBrushButton.Foreground = SystemColors.DesktopBrush;
            DesktopBrushButton.Content = SystemColors.DesktopBrush.ToString();
        }
        private void SystemGlassBrushButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(SystemGlassBrushButton.Content.ToString());
        }

        private void DesktopBrushButton_Click(object sender, RoutedEventArgs e)
        {
            DesktopBrushButton.Content = SystemColors.DesktopBrush.ToString();
            Clipboard.SetText(DesktopBrushButton.Content.ToString());
        }
        private void FetchButton_Click(object sender, RoutedEventArgs e)
        {
            ColorChanged();
        }
    }
}
