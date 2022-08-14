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
        public static PropertyClass parameterClass = new(),
            recentSettingsClass = new(),
            hangerSettingsClass = new();
        public static Window currentWindow = new();
        public static StackPanel contentStackPanel = new();
        public static MainWindow? mainWindow;
        public static bool isMainWindowClosed = false;
    }
}
