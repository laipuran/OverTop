using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static OverTop.WindowClass;

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
            await Task.Run(TrySetPosition);
        }

        private void Window_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Window chooserWindow = new ChooserWindow();
            App.contentStackPanel = ContentStackPanel;
            chooserWindow.ShowDialog();
        }

        private ScreenPart GetPart(Point point)
        {
            // Can automatically change when dpi is changed
            Point screenSize = new();
            screenSize.X = SystemParameters.PrimaryScreenWidth;
            screenSize.Y = SystemParameters.PrimaryScreenHeight;

            if (point.X < screenSize.X / 4)
            {
                return ScreenPart.LeftPart;
            }
            else if (point.X > screenSize.X * 3 / 4)
            {
                return ScreenPart.RightPart;
            }
            else if (point.Y < screenSize.Y / 2)
            {
                return ScreenPart.TopPart;
            }
            else // if (point.Y > screenSize.Y / 2)
            {
                return ScreenPart.BottomPart;
            }
        }

        private Quadrant GetQuadrant(Point point) // not used
        {
            Point middleScreen = new();
            middleScreen.X = SystemParameters.PrimaryScreenWidth / 2;
            middleScreen.Y = SystemParameters.PrimaryScreenHeight / 2;
            if (point.X > middleScreen.X && point.Y < middleScreen.Y)
            {
                return Quadrant.Quadrant1;
            }
            else if (point.X < middleScreen.X && point.Y < middleScreen.Y)
            {
                return Quadrant.Quadrant2;
            }
            else if (point.X < middleScreen.X && point.Y > middleScreen.Y)
            {
                return Quadrant.Quadrant3;
            }
            else // if (point.X > middleScreen.X && point.Y > middleScreen.Y)
            {
                return Quadrant.Quadrant4;
            }
        }
        
        private Point GetMiddlePoint()
        {
            Point point = new();
            point.X = Left + Width / 2;
            point.Y = Top + Height / 2;
            
            return point;
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
            ScreenPart part = GetPart(GetMiddlePoint());
            // TODO: Automatically change orientation and position when the middle point moves to another part of the screen

            SetWindowPos(part);
        }
    }
}
