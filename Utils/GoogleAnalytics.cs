using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Threading;
#pragma warning disable 618

namespace iMine.Launcher.Utils
{
    public static class GoogleAnalytics
    {
        private static readonly LinkedList<NameValueCollection> Requests = new LinkedList<NameValueCollection>();
        private const string Tid = "UA-117879893-1";

        public static void Init()
        {
            new Thread(Loop).Start();
        }

        public static void StartSession()
        {
            var cores = 0;
            var frequency = 0;
            var sram = 0;
            var os = "";
            try
            {
                var cpu = HardwareInfo.GetCoresAndSpeed();
                cores = cpu[0];
                frequency = cpu[1];
            } catch { }
            try { sram = HardwareInfo.GetTotalRam(); } catch { }
            try { os = HardwareInfo.GetOsVersion(); } catch { }
            var data = new NameValueCollection
            {
                {"t", "screenview"},
                {"an", "Launcher"},
                {"av", App.GetVersion()},
                {"cd", "loading"},
                {"sc", "start"},
                {"cd1", HardwareInfo.GetBits().ToString()},
                {"cd2", os},
                {"cd3", cores.ToString()},
                {"cd4", frequency.ToString()},
                {"cd5", sram.ToString()}
            };

            SendRequest(data);
        }

        public static void EndSession(string reason)
        {
            var data = new NameValueCollection
            {
                {"t", "screenview"},
                {"an", "Launcher"},
                {"av", App.GetVersion()},
                {"sc", "end"},
            };
            SendRequest(data);
        }

        public static void Event(string category, string action, string label=null, string value=null)
        {
            var data = new NameValueCollection
            {
                {"t", "event"},
                {"ec", category},
                {"ea", action},
            };
            if (label!=null)
                data.Add("el",label);
            if (value!=null)
                data.Add("ev",value);
            SendRequest(data);
        }

        public static void Timings(string category, string variable, int time, string label)
        {
            var data = new NameValueCollection
            {
                {"t", "timing"},
                {"utc", category},
                {"utv", variable},
                {"utt", time.ToString()},
                {"utl", label},
            };
            SendRequest(data);
        }

        public static void ScreenView(string name)
        {
            var data = new NameValueCollection
            {
                {"t", "screenview"},
                {"an", "Launcher"},
                {"av", App.GetVersion()},
                {"cd", name},
            };
            SendRequest(data);
        }

        public static void Exception(string name, Exception exception, string info, bool isFatal)
        {
            var data = new NameValueCollection
            {
                {"t", "exception"},
                {"exd", name},
                {"exf", isFatal ? "1" : "0"},
            };
            SendRequest(data);
            new Thread(() =>
            {
                try
                {
                    using (var client = new WebClient())
                    {
                        client.Proxy = GlobalProxySelection.GetEmptyWebProxy();
                        var stacktrace = App.GetConsoleText();
                        stacktrace=stacktrace.Substring(Math.Max(0,stacktrace.Length - 32000));
                        if (exception != null)
                            stacktrace += "\nexception:\n" + exception;
                        var exceptionData = new NameValueCollection
                        {
                            {"stacktrace", stacktrace},
                            {"errorid", name},
                            {"systeminfo", HardwareInfo.GetRawData().ToString()},
                            {"settings", Settings.InnerClone().ToString()},
                            {"uuid", Settings.GetId() ?? ""},
                            {"username", Settings.Username ?? ""},
                            {"info", info ?? ""},
                            {"isFatal", isFatal ? "1" : "0"},
                        };
                        client.UploadValues("https://imine.ru/api/exceptionreport", "POST", exceptionData);
                    }
                }
                catch
                {
                }
            }).Start();
        }

        public static bool SendBugreport(string username, string message)
        {
            App.WriteLog("Отправляем отчет об ошибке...");
            using (var client = new WebClient())
            {
                client.Proxy = GlobalProxySelection.GetEmptyWebProxy();
                var stacktrace = App.GetConsoleText();
                stacktrace=stacktrace.Substring(Math.Max(0,stacktrace.Length - 32000));
                var data = new NameValueCollection
                {
                    {"stacktrace", stacktrace},
                    {"systeminfo", HardwareInfo.GetRawData().ToString()},
                    {"settings", Settings.InnerClone().ToString()},
                    {"uuid", Settings.GetId()},
                    {"username", username ?? ""},
                    {"message", message ?? ""},
                };
                var responsebytes = client.UploadValues("https://imine.ru/api/crashreport", "POST", data);
                var responsebody = Encoding.UTF8.GetString(responsebytes);
                if (responsebody == "OK")
                {
                    App.WriteLog("Отчет об ошибке успешно отправлен!");
                    return true;
                }
            }
            return false;
        }

        private static void SendRequest(NameValueCollection data)
        {
            data.Add("v", "1");
            data.Add("tid", Tid);
            data.Add("cid", Settings.GetId());
            Requests.AddLast(data);
        }

        private static void Loop()
        {
            for (;;)
            {
                Thread.Sleep(100);
                try
                {
                    if (Requests.Count == 0)
                    {
                        if (App.IsShuttingDown)
                        {
                            Environment.Exit(0);
                            return;
                        }
                        continue;
                    }

                    /*using (var client = new WebClient())
                    {
                        var request = Requests.First.Value;
                        Requests.RemoveFirst();
                        client.Proxy = GlobalProxySelection.GetEmptyWebProxy();

                        client.UploadValues("https://www.google-analytics.com/collect", "POST", request);
                    }         */
                    Requests.RemoveFirst();
                    //Thread.Sleep(1000);
                    Thread.Sleep(100);
                }
                catch
                {
                }
            }
        }
    }
}