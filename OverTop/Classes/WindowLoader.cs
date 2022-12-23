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
        public static void OpenFromInternet(string site)
        {
            string json = API.GetData(site);
            if (json is null) return;

#pragma warning disable CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
            HangerWindowProperty windowClass= JsonConvert.DeserializeObject<Floatings.HangerWindowProperty>(json);
#pragma warning restore CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。

            FloatingPanelPage.hangers++;
            HangerWindow newHanger = (HangerWindow)windowClass.GetWindow();
            newHanger.Show();
            FloatingPanelPage.windows.Add(newHanger);
        }
    }
}
