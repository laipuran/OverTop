using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OverTop.Floatings;
using static OverTop.API;

namespace OverTop
{
    public class Settings
    {
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        public string ip;
        public IpInformation i2;
        public Property HangerWindowSetting;
        public Property RecentWindowSetting;
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

        public static Settings GetSettingsFromFile(string ip)
        {
            Settings settings = new();

            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\OverTop\\Settings.json";
            if (!File.Exists(filePath))
            {
                settings = GetDefaultSettings(ip);
                return settings;
            }
            string json = File.ReadAllText(filePath);
            try
            {
#pragma warning disable CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
                settings = JsonConvert.DeserializeObject<Settings>(json);
#pragma warning restore CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。

                if (settings is null)
                {
                    Exception ex = new PuranLai.CustomException("You are using the settings file in old version!");
                    throw ex;
                }
            }
            catch
            {
                return GetDefaultSettings(ip);
            }

            string IP = API.GetHostIp();
            if (IP != settings.ip)
                settings.i2 = API.GetIpInformation(ip);
            return settings;
        }

        public static Settings GetDefaultSettings(string ip)
        {
            Settings settings = new();

            Property hangerProperty = new(), recentProperty = new();
            hangerProperty.height = 150;
            hangerProperty.width = 200;
            hangerProperty.alpha = 0.7;
            hangerProperty.backGroundColor = System.Windows.Media.Color.FromRgb(255, 161, 46);

            recentProperty.height = 350;
            recentProperty.width = 250;
            recentProperty.alpha = 0.8;
            recentProperty.backGroundColor = System.Windows.Media.Color.FromRgb(69, 181, 255);

            settings.HangerWindowSetting = hangerProperty;
            settings.RecentWindowSetting = recentProperty;
            settings.i2 = GetIpInformation(ip);
            return settings;
        }

        public static void SaveSettings()
        {

        }
    }
}
