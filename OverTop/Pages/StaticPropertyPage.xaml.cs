using Microsoft.Win32;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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

            SystemGlassBrushButton.Foreground = SystemParameters.WindowGlassBrush;
            SystemGlassBrushButton.Content = SystemParameters.WindowGlassBrush.ToString();
            DesktopBrushButton.Foreground = SystemColors.DesktopBrush;
            DesktopBrushButton.Content = SystemColors.DesktopBrush.ToString();
            ColorChanged();
            SystemEvents.UserPreferenceChanged += UserPreferenceChanged;

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
                    string resourceName = item.ToString();
#pragma warning restore CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
                    if (resourceName == "MainColor")
                    {
                        SolidColorBrush brush = new SolidColorBrush(SystemParameters.WindowGlassColor);
                        brush.Opacity = 0.8;
                        mergedDictionaries[i][item] = brush;
                    }
                }
            }
        }
        
        private async void SystemGlassBrushButton_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() => { Clipboard.SetText(SystemGlassBrushButton.Content.ToString()); });
        }

        private async void DesktopBrushButton_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() => { Clipboard.SetText(DesktopBrushButton.Content.ToString()); });
        }

    }
}
