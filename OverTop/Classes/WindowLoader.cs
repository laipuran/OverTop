using Hardcodet.Wpf.TaskbarNotification.Interop;
using Newtonsoft.Json;
using OverTop.Floatings;
using OverTop.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OverTop
{
    internal class WindowLoader
    {
        public static HangerWindowProperty? OpenFromInternet(string site)
        {
            string? json = API.GetData(site);
            if (json is null) return null;

            HangerWindowProperty? windowClass= JsonConvert.DeserializeObject<Floatings.HangerWindowProperty>(json);

            return windowClass;
        }
    }
}
