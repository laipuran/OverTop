using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;

namespace OverTop
{
    public class API
    {
        private static string key = "ad768f25db9eb67e3883c2a16f59295b";
        private static string ipSrc = "https://ip.useragentinfo.com/myip";
        private static string amapBase = "https://restapi.amap.com/v3/";

        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class IpInformation
        {
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
            public string status { get; set; }
            public string info { get; set; }
            public string infocode { get; set; }
            public string province { get; set; }
            public string city { get; set; }
            public string adcode { get; set; }
            public string rectangle { get; set; }
        }

        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class Life
        {
            public string province { get; set; }
            public string city { get; set; }
            public string adcode { get; set; }
            public string weather { get; set; }
            public string temperature { get; set; }
            public string winddirection { get; set; }
            public string windpower { get; set; }
            public string humidity { get; set; }
            public string reporttime { get; set; }
        }

        public class WeatherInformation
        {
            public string status { get; set; }
            public string count { get; set; }
            public string info { get; set; }
            public string infocode { get; set; }
            public List<Life> lives { get; set; }
        }

#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

        public static string? GetData(string url)
        {
            string? result = null;
            try
            {
                HttpClient client = new HttpClient();
                result = client.GetStringAsync(url).Result;
            }
            catch { }
            return result;
        }

        public static string? GetHostIp()
        {
            string? ip = GetData(ipSrc);
            if (ip is not null)
                return ip.Substring(0, ip.Length);
            return null;
        }

        public static IpInformation? GetIpInformation(string ip)
        {
            string? json = GetData(amapBase + "ip?key=" + key + "&ip=" + ip);
            if (json is null) return null;
#pragma warning disable CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
            IpInformation iP = JsonConvert.DeserializeObject<IpInformation>(json);
#pragma warning restore CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。

            return iP;
        }

        public static WeatherInformation? GetWeatherInformation(string adcode)
        {
            string? json = GetData(amapBase + "weather/weatherInfo?key=" + key + "&city=" + adcode);
            if (json is null) return null;
#pragma warning disable CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
            WeatherInformation weather = JsonConvert.DeserializeObject<WeatherInformation>(json);
#pragma warning restore CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。

            return weather;
        }
    }
}
