﻿using Newtonsoft.Json;
using PuranLai.Algorithms;
using System;
using System.Collections.Generic;
using System.IO;
using static OverTop.WebApi;

namespace OverTop
{
    interface ISettings
    {
        void Save();
    }

    public class Settings : ISettings
    {
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        public string? IP;
        public IpInformation? ipInformation;
        public WindowProperty HangerWindowSettings;
        public WindowProperty RecentWindowSettings;
        public List<HangerWindowProperty>? HangerWindows = new();
        public RecentWindowProperty? RecentWindow;
        public WeatherWindowProperty WeatherWindow;
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

        public static Settings GetSettings(string ip)
        {
            Settings settings = new();

            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\OverTop\\Settings.json";
            if (!File.Exists(filePath))
            {
                settings = GetDefaultSettings(ip);

                HangerWindowProperty? welcomeWindow = WindowLoader.OpenFromInternet("https://laipuran.github.io/Products/OverTop/welcome.json");

                if (welcomeWindow is not null)
                {
                    settings.HangerWindows = new();
                    settings.HangerWindows.Add(welcomeWindow);
                }
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
                    Exception ex = new CustomException("The Settings file you are using is in old version.");
                    throw ex;
                }
            }
            catch
            {
                return GetDefaultSettings(ip);
            }

            if (ip != settings.IP || settings.ipInformation is null)
            {
                settings.ipInformation = WebApi.GetIpInformation(ip);
                settings.IP = ip;
            }

            return settings;
        }

        public static Settings GetDefaultSettings(string? ip)
        {
            Settings settings = new();

            WindowProperty hanger = new(), recent = new();
            hanger.Height = 150;
            hanger.Width = 200;
            hanger.BackgroundColor = System.Windows.Media.Color.FromRgb(102, 111, 255);

            recent.Height = 350;
            recent.Width = 250;
            recent.BackgroundColor = System.Windows.Media.Color.FromRgb(69, 181, 255);

            settings.HangerWindowSettings = hanger;
            settings.RecentWindowSettings = recent;
            if (ip is not null)
                settings.ipInformation = GetIpInformation(ip);
            settings.IP = ip;
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
