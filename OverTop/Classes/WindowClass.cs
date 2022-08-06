using System.Collections.Generic;

namespace OverTop
{
    internal class HangerWindowClass
    {
        public enum ContentType
        {
            Text,
            Image
        }

        public Dictionary<ContentType, string> contents = new();
        public string backgroundColor = "";
        public int width;
        public int height;
        public double alpha;
        public double left;
        public double top;

    }

    internal class AppWindowClass
    {
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
    }
}
