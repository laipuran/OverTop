﻿using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace OverTop.Pages
{
    /// <summary>
    /// FloatingPanelPage.xaml 的交互逻辑
    /// </summary>
    public partial class FloatingPanelPage : Page
    {
        List<Window> windows = new List<Window>();
        public FloatingPanelPage()
        {
            InitializeComponent();
        }
        private void RecentWindowButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Window newRecent = new Floatings.RecentWindow();
            newRecent.Show();
            windows.Add(newRecent);
        }
        private void HangerWindowButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Window newHanger = new Floatings.HangerWindow();
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
