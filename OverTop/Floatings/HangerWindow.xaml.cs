using Newtonsoft.Json;
using OverTop.Pages;
using PuranLai.Algorithms;
using PuranLai.Tools;
using PuranLai.Tools.Classes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static OverTop.CommonWindowOps;

namespace OverTop.Floatings
{
    /// <summary>
    /// HangerWindow.xaml 的交互逻辑
    /// </summary>
    public partial class HangerWindow : Window
    {
        public bool isMouseIn = false;
        List<KeyValuePair<HangerWindowProperty, string>> contents = new();

        private static bool isBottom = false;

        public HangerWindow(HangerWindowProperty property)
        {
            InitializeComponent();

            FloatingPanelPage.hangers++;
            FloatingPanelPage.windows.Add(this);

            this.Top = property.top == 0 ? Top : property.top;
            this.Left = property.left == 0 ? Left : property.left;
            this.Width = property.width;
            this.Height = property.height;
            System.Drawing.Color tempColor = ColorTranslator.FromHtml(property.backgroundColor);
            System.Windows.Media.Color color = System.Windows.Media.Color.FromRgb(tempColor.R, tempColor.G, tempColor.B);
            this.Background = new SolidColorBrush(color);
            this.Opacity = property.alpha == 0.0 ? 0.8 : App.settings.HangerWindowSettings.alpha;
            this.ToolTip = "Hanger Window - " + FloatingPanelPage.hangers;
            this.Title = property.guid;
            if (!property.isTop)
            {
                isBottom = ChangeStatus(false, this);
            }
            StackPanel ContentStackPanel = (StackPanel)((ScrollViewer)this.Content).Content;
            foreach (KeyValuePair<HangerWindowProperty.ContentType, string> pair in property.contents)
            {
                if (pair.Key == HangerWindowProperty.ContentType.Text)
                {
                    TextBlock newTextBlock = new();
                    newTextBlock.Style = (Style)FindResource("ContentTextBlockStyle");
                    newTextBlock.Text = pair.Value;
                    StackPanel newStackPanel = new();
                    newStackPanel.Children.Add(newTextBlock);
                    newStackPanel.MouseLeftButtonDown += TextPanel_MouseLeftButtonDown;
                    ContentStackPanel.Children.Add(newStackPanel);
                }
                else if (pair.Key == HangerWindowProperty.ContentType.Image)
                {
                    try
                    {
                        System.Windows.Controls.Image newImage = new();
                        newImage.Source = new BitmapImage(new Uri(pair.Value));
                        StackPanel newStackPanel = new();
                        newStackPanel.Children.Add(newImage);
                        newStackPanel.MouseLeftButtonDown += NewStackPanel_MouseLeftButtonDown;
                        ContentStackPanel.Children.Add(newStackPanel);
                    }
                    catch
                    {
                        throw new CustomException("Image not exists!");
                    }
                }
            }
        }

        private void NewStackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.R))
            {
                ((StackPanel)((StackPanel)sender).Parent).Children.Remove((StackPanel)sender);
            }
        }

        public void TextPanel_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            StackPanel currentStackPanel = (StackPanel)sender;
            TextBlock currentTextBlock = (TextBlock)currentStackPanel.Children[0];
            if (Keyboard.IsKeyDown(Key.M))
            {
                TextWindow textWindow = new("请输入文本：", currentTextBlock.Text);
                textWindow.ShowDialog();
                currentTextBlock.Text = textWindow.result;
            }
            else if (Keyboard.IsKeyDown(Key.R))
            {
                ((StackPanel)currentStackPanel.Parent).Children.Remove(currentStackPanel);
            }
        }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (System.Windows.Input.Mouse.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Tab)
            {
                isBottom = CommonWindowOps.ChangeStatus(isBottom, this);
                return;
            }
            try
            {
                switch (e.Key)
                {
                    case Key.Left: Left -= 1; break;
                    case Key.Right: Left += 1; break;
                    case Key.Up: Top -= 1; break;
                    case Key.Down: Top += 1; break;
                    case Key.Escape: Close(); break;
                    default: break;
                        //TODO: Fix needed
                }
            }
            catch { }
        }

        private void ContentStackPanel_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.LeftCtrl))
            {
                string json = JsonConvert.SerializeObject(this.Save());
                string filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\OverTop\\HangerWindows\\" + Title + ".json";
#pragma warning disable CS8602 // 解引用可能出现空引用。
                Directory.CreateDirectory(Directory.GetParent(filePath).FullName);
#pragma warning restore CS8602 // 解引用可能出现空引用。
                File.WriteAllText(filePath, json);
            }
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private unsafe void Window_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            CommonWindowOps.ChangeZIndex(isBottom, this);
            Scroller.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            fixed (bool* MouseIn = &isMouseIn)
            {
                WindowOperations.ToWhite(MouseIn, this);
            }
        }

        private unsafe void Window_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            CommonWindowOps.ChangeZIndex(isBottom, this);
            Scroller.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            fixed (bool* MouseIn = &isMouseIn)
            {
                WindowOperations.ToTransparent(MouseIn, this);
            }
        }

        private void Window_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            App.currentWindow = this;
            App.contentStackPanel = ContentStackPanel;
            App.currentWindowType = CommonWindowOps.WindowType.Hanger;
            Window propertyWindow = new PropertyWindow
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            propertyWindow.ShowDialog();
            Width = App.tempProperty.width;
            Height = App.tempProperty.height;
            Background = new SolidColorBrush(App.tempProperty.backGroundColor);
            Opacity = App.tempProperty.alpha;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            FloatingPanelPage.hangers--;
            FloatingPanelPage.windows.Remove(this);
        }
    }
}
