using System.Windows;
using System.Windows.Controls;

namespace OverTop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ParameterClass parameterClass = new();
        public static Window currentWindow = new();
        public static StackPanel contentStackPanel = new();
    }
}
