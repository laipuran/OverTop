using System.Threading.Tasks;
using System.Timers;
using System.Windows;

namespace OverTop.FunctionalWindows
{
    /// <summary>
    /// SplashWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SplashWindow : Window
    {
        public SplashWindow()
        {
            InitializeComponent();
            Task.Run(Wait);
        }

        public void Wait()
        {
            Timer timer = new(3000);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                this.Close();
            });
        }
    }
}
