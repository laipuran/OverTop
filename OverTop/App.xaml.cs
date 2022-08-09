﻿using System.Windows;
using System.Windows.Controls;

namespace OverTop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public enum WindowType
        {
            Hanger = 0,
            Recent = 1
        }
        public static WindowType windowType = new();
        public static ParameterClass parameterClass = new();
        public static Window currentWindow = new();
        public static StackPanel contentStackPanel = new();
        public static MainWindow? mainWindow;
    }
}
