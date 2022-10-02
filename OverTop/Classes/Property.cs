using Hardcodet.Wpf.TaskbarNotification.Interop;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Media;

namespace OverTop.Floatings
{
    interface IProperty
    {
        Window GetWindow();
    }

    public class Property
    {
        public System.Windows.Media.Color backGroundColor;
        public double alpha;
        public int width;
        public int height;
    }

    public class WeatherWindowProperty : IProperty
    {
        public int width;
        public int height;
        public double left;
        public double top;
        public bool isVisible;

        public Window GetWindow()
        {
            WeatherWindow window = new();
            window.Width = this.width;
            window.Height = this.height;
            window.Left = this.left;
            window.Top = this.top;
            // TODO: Add file name
            return window;
        }
    }

    public class RecentWindowProperty
    {
        public string backgroundColor = "";
        public int width;
        public int height;
        public double alpha;
        public double left;
        public double top;

        public Window GetWindow()
        {
            RecentWindow window = new();
            window.Width = this.width;
            window.Height = this.height;
            window.Left = this.left;
            window.Top = this.top;
            System.Drawing.Color tempColor = ColorTranslator.FromHtml(this.backgroundColor);
            window.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(tempColor.R, tempColor.G, tempColor.B));
            return window;
        }
    }

    public class HangerWindowProperty
    {
        public List<KeyValuePair<ContentType, string>> contents = new();
        public string backgroundColor = "";
        public int width;
        public int height;
        public double alpha;
        public double left;
        public double top;

        public enum ContentType
        {
            Text,
            Image
        }

        public Window GetWindow()
        {
            RecentWindow window = new();
            window.Width = this.width;
            window.Height = this.height;
            window.Left = this.left;
            window.Top = this.top;
            System.Drawing.Color tempColor = ColorTranslator.FromHtml(this.backgroundColor);
            window.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(tempColor.R, tempColor.G, tempColor.B));
            // TODO: Move methods to add new hanger window here
            return window;
        }
    }
}
