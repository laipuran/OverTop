using System.Collections.Generic;
using System.Windows.Media;

namespace OverTop
{
    internal class WindowClass
    {
        public enum ContentType
        {
            Text = 1,
            Image = 2
        }
        public Dictionary<ContentType, string> contents = new();
        public string backgroundColor = "";
        public int width;
        public int height;
        public double alpha;
        public double left;
        public double top;

    }
}
