using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OverTop.Floatings
{
    /// <summary>
    /// ChooserWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ChooserWindow : Window
    {
        string programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu);
        public ChooserWindow()
        {
            InitializeComponent();

            Task.Run(() => Dispatcher.BeginInvoke(new Action(GetShortCuts)));
        }

        private void GetShortCuts()
        {
            List<string> filePath = new();
            Dictionary<string, Bitmap> fileInfo = new();
            foreach (string file in Directory.GetFiles(programData))
            {
                filePath.Add(file);
            }
            
            foreach(string folder in Directory.GetDirectories(programData))
            {
                foreach (string file in Directory.GetFiles(folder))
                {
                    filePath.Add(file);
                }
            }
            
            foreach (string file in filePath)
            {
                if (!file.EndsWith(".lnk"))
                {
                    continue;
                }
                IWshRuntimeLibrary.WshShell shell = new();
                IWshRuntimeLibrary.IWshShortcut wshShortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(file);

#pragma warning disable CS8602 // 解引用可能出现空引用。
                Bitmap icon = System.Drawing.Icon.ExtractAssociatedIcon(wshShortcut.TargetPath).ToBitmap();
#pragma warning restore CS8602 // 解引用可能出现空引用。
                fileInfo.Add(wshShortcut.TargetPath, icon);

                foreach (KeyValuePair<string, Bitmap> keyPair in fileInfo)
                {
                    Thickness margin = new(10, 0, 0, 0);
                    Thickness margin1 = new(5, 5, 5, 5);
                    StackPanel newStackPanel = new()
                    {
                        Height = 30,
                        Margin = margin1,
                        Orientation = Orientation.Horizontal
                    };
                    System.Windows.Controls.Image image = new();
                    BitmapSource bitmapSource = System.Windows.Interop.Imaging
                        .CreateBitmapSourceFromHBitmap(keyPair.Value.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    image.Source = bitmapSource;
                    TextBlock textBlock = new()
                    {
                        Text = System.IO.Path.GetFileName(keyPair.Key),
                        Style = (Style)FindResource("ContentTextBlockStyle"),
                        Margin = margin
                    };
                    newStackPanel.Children.Add(image);
                    newStackPanel.Children.Add(textBlock);

                    newStackPanel.MouseLeftButtonDown += NewStackPanel_MouseLeftButtonDown;

                    ContentStackPanel.Children.Add(newStackPanel);
                }
            }
            
        }

        private void NewStackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StackPanel appPanel = new();
            System.Windows.Controls.Image image = ((System.Windows.Controls.Image)((StackPanel)sender).Children[0]);
            appPanel.Children.Add(image);
            appPanel.ToolTip = ((TextBlock)((StackPanel)sender).Children[1]).Text;
            appPanel.MouseLeftButtonDown += AppPanel_MouseLeftButtonDown;
            App.contentStackPanel.Children.Add(appPanel);
        }

        private void AppPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
#pragma warning disable CS8604 // 引用类型参数可能为 null。
            Process.Start("explorer.exe", ((StackPanel)sender).ToolTip.ToString());
#pragma warning restore CS8604 // 引用类型参数可能为 null。
        }
    }
}
