using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace OverTop.Pages
{
    /// <summary>
    /// FloatingPanelPage.xaml 的交互逻辑
    /// </summary>
    public partial class FloatingPanelPage : Page
    {
        public static List<Window> windows = new();
        public static int recents = 0, hangers = 0;
        public FloatingPanelPage()
        {
            InitializeComponent();
        }
        private void RecentWindowButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            recents++;
            Window newRecent = new Floatings.RecentWindow();
            newRecent.Title = "Recent Window - " + recents;
            newRecent.ToolTip = newRecent.Title;
            newRecent.Show();
            windows.Add(newRecent);
        }
        private void HangerWindowButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            hangers++;
            Window newHanger = new Floatings.HangerWindow();
            newHanger.Title = "Hanger Window - " + hangers;
            newHanger.ToolTip = newHanger.Title;
            newHanger.Show();
            windows.Add(newHanger);
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (Window window in windows)
            {
                window.Close();
            }
        }

    }
}
