using System;
using System.Collections.Generic;
using System.Windows;

namespace OverTop.Floatings
{
    interface IProperty
    {
        Window GetWindow();
        void FromProperty(WindowProperty property);
    }

    public class WindowProperty
    {
        public System.Windows.Media.Color backGroundColor;
        public int width;
        public int height;
    }

    public class WeatherWindowProperty : IProperty
    {
        public double left;
        public double top;
        public bool isVisible;

        public Window GetWindow()
        {
            if (this is null)
            {
                return GetDefaultProperty().GetWindow();
            }
            return new WeatherWindow(this);
        }

        public void FromProperty(WindowProperty property)
        {
            // Empty
        }

        public static WeatherWindowProperty GetDefaultProperty()
        {
            WeatherWindowProperty property = new();
            property.left = 30;
            property.top = 30;
            property.isVisible = false;
            return property;
        }
    }

    public class RecentWindowProperty : IProperty
    {
        public string backgroundColor = "";
        public int width;
        public int height;
        public double left;
        public double top;
        public bool isTop;

        public Window GetWindow()
        {
            if (this is null)
            {
                return GetDefaultProperty().GetWindow();
            }
            return new RecentWindow(this);
        }

        public void FromProperty(WindowProperty property)
        {
            this.width = property.width;
            this.height = property.height;
            System.Windows.Media.Color tempColor = property.backGroundColor;
            this.backgroundColor = System.Drawing.ColorTranslator.ToHtml(System.Drawing.Color.FromArgb(tempColor.R, tempColor.G, tempColor.B));
        }

        public static RecentWindowProperty GetDefaultProperty()
        {
            RecentWindowProperty property = new();
            property.FromProperty(App.settings.RecentWindowSettings);
            property.isTop = true;
            return property;
        }
    }

    public class HangerWindowProperty : IProperty
    {
        public List<KeyValuePair<ContentType, string>> contents = new();
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        public string backgroundColor;
        public int width;
        public int height;
        public double alpha;
        public double left;
        public double top;
        public string guid;
        public bool isTop;
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

        public enum ContentType
        {
            Text,
            Image
        }

        public void FromProperty(WindowProperty property)
        {
            this.width = property.width;
            this.height = property.height;
            System.Windows.Media.Color tempColor = property.backGroundColor;
            this.backgroundColor = System.Drawing.ColorTranslator.ToHtml(System.Drawing.Color.FromArgb(tempColor.R, tempColor.G, tempColor.B));
        }

        public static HangerWindowProperty GetDefaultProperty()
        {
            HangerWindowProperty property = new();
            property.FromProperty(App.settings.HangerWindowSettings);
            property.guid = Guid.NewGuid().ToString();
            property.isTop = true;
            return property;
        }

        public Window GetWindow()
        {
            if (this is null)
            {
                return GetDefaultProperty().GetWindow();
            }
            return new HangerWindow(this);
        }
    }
}
