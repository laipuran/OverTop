using Newtonsoft.Json;
using System.Windows.Controls;

namespace OverTop
{
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
