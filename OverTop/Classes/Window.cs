using Newtonsoft.Json;

namespace OverTop
{
    public class WindowLoader
    {
        public static HangerWindowProperty? OpenFromInternet(string site)
        {
            string? json = WebApi.GetData(site);
            if (json is null) return null;

            HangerWindowProperty? windowClass = JsonConvert.DeserializeObject<HangerWindowProperty>(json);

            return windowClass;
        }
    }
}
