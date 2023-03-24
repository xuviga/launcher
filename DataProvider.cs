using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;
using iMine.Launcher.Client;
using iMine.Launcher.Request.Auth;
using iMine.Launcher.Utils;
using iMine.Launcher.Utils.Collections;
using Newtonsoft.Json.Linq;

namespace iMine.Launcher
{
    public static class DataProvider
    {
        public static NDictionary<string, ServerInfo> Servers = new NDictionary<string, ServerInfo>();
        public static NDictionary<string, string> Links = new NDictionary<string, string>();

        public static ServerInfo SelectedServer;
        public static Thread DownloadThread;
        public static Process GameProcess;
        public static AuthRequest.Result AuthResult;

        public static void DoEveryfin()
        {
            var links = new NDictionary<string, string>
            {
                ["Наш Discord"] = "https://imine.ru/discord",
                ["Группа ВК"] = "https://vk.com/imineru",
                ["Наш сайт"] = "https://imine.ru"
            };
            App.WriteLog("Загружаем данные по серверам...");
            var profiles = Config.ConnectLauncher();
            var servers = new NDictionary<string, ServerInfo>();
            var processed = new HashSet<string>();
            foreach (var profileFor in profiles)
            {
                var profile = profileFor;

                try
                {
                    servers[profile.GetTitle()] = new ServerInfo(profile);
                    var result1 = new string[1];
                    var result2 = new string[1];
                    var result3 = new string[1];

                    var client1 = new WebClient();
                    var client2 = new WebClient();
                    var client3 = new WebClient();

                    App.WriteLog($"[{profile.GetTitle()}] Загружаем инфо сервера...");
                    client1.Proxy = GlobalProxySelection.GetEmptyWebProxy();
                    client2.Proxy = GlobalProxySelection.GetEmptyWebProxy();
                    client3.Proxy = GlobalProxySelection.GetEmptyWebProxy();

                    new Thread(() => DownloadWebData(client1, profile,"server", result1)).Start();
                    new Thread(() => DownloadWebData(client2, profile,"news", result2)).Start();
                    new Thread(() => DownloadWebData(client3, profile,"changelogs", result3)).Start();

                    for(;;)
                    {
                        Thread.Sleep(100);
                        if (result1[0] == "nope" || result2[0] == "nope" || result3[0] == "nope")
                            throw new Exception("Не удалось загрузить данные по серверу " + profile.GetTitle());
                        if (result1[0] != null && result2[0] != null && result3[0] != null)
                        {
                            servers[profile.GetTitle()] = new ServerInfo(profile,
                                JObject.Parse(result1[0]), JArray.Parse(result2[0]), JArray.Parse(result3[0]));
                            processed.Add(profile.GetTitle());
                            break;
                        }
                    }

                }
                catch (Exception e)
                {
                    if (!profile.IsPrivate())
                    {
                        App.WriteException(e, profile.GetTitle());
                        GoogleAnalytics.Exception("info_download", null, profile.GetTitle(), true);
                    }

                    processed.Add(profile.GetTitle());
                }
            }
            while (profiles.Count > processed.Count) { Thread.Sleep(100); }

            App.WriteLog("Данные по серверам загружены!");
            Servers = servers;
            Links = links;
        }

        private static void DownloadWebData(WebClient client, ClientProfile profile, string part, string[] result)
        {
            for (var i=0;i<5;i++)
            {
                try
                {
                    var rawBytes = client.DownloadData(new Uri(Config.OurWebsite, "api/" + profile.GetTitle().ToLower() + "/" + part));
                    var webData = Encoding.UTF8.GetString(rawBytes);
                    result[0] = webData;
                    return;
                }
                catch
                {
                }
            }

            result[0] = "nope";
        }

        public static bool IsGameInProgress()
        {
            return DownloadThread != null && DownloadThread.IsAlive
                   || GameProcess != null && !GameProcess.HasExited;
        }
    }
}