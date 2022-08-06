using System.Collections.Generic;

namespace OverTop
{
    internal class WindowClass
    {
        public enum ContentType
        {
            Text,
            Image
        }
        
        public enum Quadrant // not used
        {
            Quadrant1,
            Quadrant2,
            Quadrant3,
            Quadrant4
        }
        
        public enum ScreenPart
        {
            TopPart,
            BottomPart,
            RightPart,
            LeftPart
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
