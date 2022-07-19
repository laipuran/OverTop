using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
            ColorChanged();
            SystemEvents.UserPreferenceChanged += UserPreferenceChanged;
            
            SystemGlassBrushButton.Foreground = SystemParameters.WindowGlassBrush;
            SystemGlassBrushButton.Content = SystemParameters.WindowGlassBrush.ToString();
            DesktopBrushButton.Foreground = SystemColors.DesktopBrush;
            DesktopBrushButton.Content = SystemColors.DesktopBrush.ToString();
        }
        public void UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            SystemGlassBrushButton.Background = SystemParameters.WindowGlassBrush;
            SystemGlassBrushButton.Content = SystemParameters.WindowGlassBrush.ToString();
            DesktopBrushButton.Background = SystemColors.DesktopBrush;
            DesktopBrushButton.Content = SystemColors.DesktopBrush.ToString();
            ColorChanged();
        }
        public static void ColorChanged()
        {
            var mergedDictionaries = Application.Current.Resources.MergedDictionaries;
            for (int i = 0; i < mergedDictionaries.Count; i++)
            {
                foreach (var item in mergedDictionaries[i].Keys)
                {
#pragma warning disable CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
                    string c = item.ToString();
#pragma warning restore CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
                    if (c == "MainColor")
                    {
                        SolidColorBrush brush = new SolidColorBrush(SystemParameters.WindowGlassColor);
                        brush.Opacity = 0.8;
                        mergedDictionaries[i][item] = brush;
                    }
                }
            }
        }
        private void SystemGlassBrushButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(SystemGlassBrushButton.Content.ToString());
        }

        private void DesktopBrushButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(DesktopBrushButton.Content.ToString());
        }

    }
}
