using Newtonsoft.Json;
using OverTop.Floatings;

namespace OverTop
{
    internal class WindowLoader
    {
        public static HangerWindowProperty? OpenFromInternet(string site)
        {
            string? json = API.GetData(site);
            if (json is null) return null;

            HangerWindowProperty? windowClass = JsonConvert.DeserializeObject<Floatings.HangerWindowProperty>(json);

            return windowClass;
        }
    }
}
