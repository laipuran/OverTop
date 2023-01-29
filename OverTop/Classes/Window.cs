using Newtonsoft.Json;
using System.Windows.Controls;

namespace OverTop
{
    // TODO: Do this damn ContentWindow Class
    public interface IContentWindow
    {
        public ContentWindowProperty Property { get; set; }
        public StackPanel ContentPanel { get; set; }
    }

    public class WindowLoader
    {
        public static HangerWindowProperty? OpenFromInternet(string site)
        {
            string? json = API.GetData(site);
            if (json is null) return null;

            HangerWindowProperty? windowClass = JsonConvert.DeserializeObject<HangerWindowProperty>(json);

            return windowClass;
        }
    }
}
