using Microsoft.Win32;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OverTop.Pages
{
    /// <summary>
    /// StaticPropertyPage.xaml 的交互逻辑
    /// </summary>
    public partial class PropertyPage : Page
    {
        public PropertyPage()
        {
            InitializeComponent();

            ChangeContent();
            ColorChanged();
            SystemEvents.UserPreferenceChanged += UserPreferenceChanged;

        }

        public void UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            ChangeContent();
            ColorChanged();
        }

        private void ChangeContent()
        {
            SystemGlassBrushButton.Foreground = SystemParameters.WindowGlassBrush;
            SystemGlassBrushButton.Content = SystemParameters.WindowGlassBrush.ToString();
            DesktopBrushButton.Foreground = SystemColors.DesktopBrush;
            DesktopBrushButton.Content = SystemColors.DesktopBrush.ToString();
            FontButton.Content = SystemFonts.CaptionFontFamily.ToString();
            FontButton.FontFamily = SystemFonts.CaptionFontFamily;
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
            await Task.Run(() => Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    Clipboard.SetText(SystemGlassBrushButton.Content.ToString());
                }
                catch
                {
                    整花活();
                }
            })));
        }

        private async void DesktopBrushButton_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() => Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    Clipboard.SetText(DesktopBrushButton.Content.ToString());
                }
                catch
                {
                    整花活();
                }
            })));
        }

        private async void FontButton_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() => Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    Clipboard.SetText(FontButton.Content.ToString());
                }
                catch
                {
                    整花活();
                }
            })));
        }

        private void 整花活()
        {
            Random random = new();
            switch (random.Next(1, 4))
            {
                case 1:
                    MessageBox.Show("你小子手速还挺快", "Over Top");
                    break;
                case 2:
                    MessageBox.Show("点的很快，下次不要再点了", "Over Top");
                    break;
                case 3:
                    MessageBox.Show("就你CPS能到7是吧", "Over Top");
                    break;
                default:
                    break;
            }
        }
    }
}
