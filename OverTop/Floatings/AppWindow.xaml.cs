using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;

namespace OverTop.Floatings
{
    /// <summary>
    /// AppWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AppWindow : Window
    {
        public AppWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.C))
            {
                ContentStackPanel.Children.Clear();
                return;
            }
            Window chooserWindow = new ChooserWindow();
            App.contentStackPanel = ContentStackPanel;
            chooserWindow.ShowDialog();
        }
    }
}
