using OverTop.Floatings;
using System;
using System.Reflection;
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
            bool ret;
            Mutex mutex = new(true, "MyApp", out ret);
            if (!ret)
            {
                MessageBox.Show("程序已经打开");
                App.Current.Shutdown();
            }
        }
    }
}
