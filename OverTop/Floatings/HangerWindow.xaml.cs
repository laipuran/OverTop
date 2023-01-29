using Newtonsoft.Json;
using PuranLai.Tools;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static OverTop.CommonWindowOps;

namespace OverTop.Floatings
{
    /// <summary>
    /// HangerWindow.xaml 的交互逻辑
    /// </summary>
    public partial class HangerWindow : Window
    {

        public bool isMouseIn = false;
        public HangerWindowProperty Property { get; set; }
        public StackPanel ContentPanel { get; set; }

        public HangerWindow(HangerWindowProperty property)
        {
            InitializeComponent();
            Property = property;
            App.FloatingWindows.Add(this);
            this.ToolTip = "Hanger Window - " + this.Property.guid;

            if (!Property.isTop)
            {
                Property.isTop = ChangeStatus(Property.isTop, this);
            }
            ContentPanel = ContentStackPanel;

            this.Reload();
        }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.LeftCtrl))
            {
                string json = JsonConvert.SerializeObject(Property);
                string filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\OverTop\\HangerWindows\\" + Title + ".json";
                DirectoryInfo? info = Directory.GetParent(filePath);
                if (info is null)
                    return;
                Directory.CreateDirectory(info.FullName);
                File.WriteAllText(filePath, json);
            }
            else if (Keyboard.IsKeyDown(Key.Left))
                Left -= 5;
            else if (Keyboard.IsKeyDown(Key.Right))
                Left += 5;
            else if (Keyboard.IsKeyDown(Key.Up))
                Top -= 5;
            else if (Keyboard.IsKeyDown(Key.Down))
                Top += 5;
            else if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Tab)
            {
                Property.isTop = CommonWindowOps.ChangeStatus(Property.isTop, this);
                return;
            }
        }

        private unsafe void Window_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            CommonWindowOps.ChangeZIndex(Property.isTop, this);
            fixed (bool* MouseIn = &isMouseIn)
            {
                this.ChangeOpacity(ExtendedWindowOps.OpacityOptions._75, MouseIn);
            }
        }

        private unsafe void Window_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            CommonWindowOps.ChangeZIndex(Property.isTop, this);
            fixed (bool* MouseIn = &isMouseIn)
            {
                if (this.Property.contents.Count == 0)
                    this.ChangeOpacity(5, MouseIn);
                else
                    this.ChangeOpacity(ExtendedWindowOps.OpacityOptions._25, MouseIn);
            }
            Property.top = Top;
            Property.left = Left;
        }

        private void Window_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            PropertyWindow propertyWindow = new(this)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            propertyWindow.ShowDialog();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            App.FloatingWindows.Remove(this);
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            this.Property.top = Top;
            this.Property.left = Left;
        }

        private void Scroller_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DragMove();
            if (!string.IsNullOrEmpty(this.Property.link))
                Process.Start("explorer.exe", this.Property.link);
        }
    }
}
