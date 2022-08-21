using OverTop.Floatings;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace OverTop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        private extern static IntPtr FindWindow(string? lpClassName, string? lpWindowName);
        
        public static WindowClass.WindowType windowType = new();
        public static Window currentWindow = new();
        public static StackPanel contentStackPanel = new();
        public static MainWindow? mainWindow;
        public static PropertyClass parameterClass = new(),
            recentSettingsClass = new(),
            hangerSettingsClass = new();
        public static AppWindow appWindow = new Floatings.AppWindow();
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            IntPtr hWnd = FindWindow(null, "Over Top");
            if (hWnd != IntPtr.Zero)
            {
                MessageBox.Show("Over Top 已经打开！", "Over Top");
                Environment.Exit(-1);
            }
        }
    }
}
