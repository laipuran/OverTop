using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using static OverTop.AppWindowClass;

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
        
        private async void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.C))
            {
                ContentStackPanel.Children.Clear();
                return;
            }
            DragMove();
            await Task.Run(() => Dispatcher.BeginInvoke(new Action(TrySetPosition)));
        }

        private void Window_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Window chooserWindow = new ChooserWindow();
            App.contentStackPanel = ContentStackPanel;
            chooserWindow.ShowDialog();
        }

        private void SetWindowPos(ScreenPart part)
        {
            if (part == ScreenPart.TopPart)
            {
                Height = 60;
                Width = 560;
                ContentStackPanel.Orientation = System.Windows.Controls.Orientation.Horizontal;
                Top = 0;
                Left = (SystemParameters.FullPrimaryScreenWidth - Width) / 2;
            }
            else if (part == ScreenPart.BottomPart)
            {
                Height = 60;
                Width = 560;
                ContentStackPanel.Orientation = System.Windows.Controls.Orientation.Horizontal;
                Top = SystemParameters.FullPrimaryScreenHeight - Height;
                Left = (SystemParameters.FullPrimaryScreenWidth - Width) / 2;
            }
            else if (part == ScreenPart.LeftPart)
            {
                Height = 560;
                Width = 60;
                ContentStackPanel.Orientation = System.Windows.Controls.Orientation.Vertical;
                Top = (SystemParameters.FullPrimaryScreenHeight - Height) / 2;
                Left = 0;
            }
            else
            {
                Height = 560;
                Width = 60;
                ContentStackPanel.Orientation = System.Windows.Controls.Orientation.Vertical;
                Top = (SystemParameters.FullPrimaryScreenHeight - Height) / 2;
                Left = SystemParameters.FullPrimaryScreenWidth - Width;
            }
        }
        private void TrySetPosition()
        {
            while (true)
            {
                if (Mouse.LeftButton == MouseButtonState.Released)
                {
                    break;
                }
            }
            ScreenPart part = GetPart(GetMiddlePoint(this));
            // TODO: Automatically change orientation and position when the middle point moves to another part of the screen

            SetWindowPos(part);
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
#pragma warning disable CS8602 // 解引用可能出现空引用。
            App.mainWindow.Visibility = Visibility.Visible;
            App.mainWindow.Focus();
#pragma warning restore CS8602 // 解引用可能出现空引用。
        }
    }
}
