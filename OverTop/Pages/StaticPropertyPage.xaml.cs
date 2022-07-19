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
        }
        public void UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            ColorChanged();
        }
        private void ColorChanged()
        {
            SystemGlassBrushButton.Background = SystemParameters.WindowGlassBrush;
            SystemGlassBrushButton.Content = SystemParameters.WindowGlassBrush.ToString();
            DesktopBrushButton.Background = SystemColors.DesktopBrush;
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

        private void SetOnceButton_Click(object sender, RoutedEventArgs e)
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
                        mergedDictionaries[i][item] = new SolidColorBrush(SystemParameters.WindowGlassColor);
                    }
                }
            }
        }
    }
}
