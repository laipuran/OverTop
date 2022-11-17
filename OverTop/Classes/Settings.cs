﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms.ComponentModel.Com2Interop;
using Microsoft.Win32;
using Newtonsoft.Json;
using OverTop.Floatings;
using static OverTop.API;

namespace OverTop
{
    interface ISettings
    {
        void Save();
    }

    public class Settings : ISettings
    {
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        public string? ip;
        public IpInformation? i2;
        public Property HangerWindowSettings;
        public Property RecentWindowSettings;
        public List<HangerWindowProperty>? HangerWindows = new();
        public RecentWindowProperty? RecentWindow;
        public WeatherWindowProperty WeatherWindow;
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

            if (ip != settings.ip && ip is not null)
            {
                settings.i2 = API.GetIpInformation(ip);
                settings.ip = ip;
            }

            return settings;
        }

        public static Settings GetDefaultSettings(string? ip)
        {
            Settings settings = new();

            Property hanger = new(), recent = new();
            hanger.height = 150;
            hanger.width = 200;
            hanger.alpha = 0.7;
            hanger.backGroundColor = System.Windows.Media.Color.FromRgb(102, 111, 255);

            recent.height = 350;
            recent.width = 250;
            recent.alpha = 0.8;
            recent.backGroundColor = System.Windows.Media.Color.FromRgb(69, 181, 255);

            settings.HangerWindowSettings = hanger;
            settings.RecentWindowSettings = recent;
            if (ip is not null)
                settings.i2 = GetIpInformation(ip);
            settings.ip = ip;
            return settings;
        }

        public void Save()
        {
            string json = JsonConvert.SerializeObject(this);
            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\OverTop\\Settings.json";
#pragma warning disable CS8602 // 解引用可能出现空引用。
            Directory.CreateDirectory(Directory.GetParent(filePath).FullName);
#pragma warning restore CS8602 // 解引用可能出现空引用。
            File.WriteAllText(filePath, json);
        }
    }
}
