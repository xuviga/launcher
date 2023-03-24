using System;
using System.IO;
using System.Threading;
using iMine.Launcher.Client;
using iMine.Launcher.Request.Auth;
using iMine.Launcher.Utils;
using iMine.Launcher.Windows;
using Newtonsoft.Json.Linq;

namespace iMine.Launcher
{
    public static class Settings
    {
        private static readonly JObject Inner;
        private static readonly FileInfo SettingsFile;

        static Settings()
        {
            SettingsFile = Config.WorkingDir.ResolveFile("Launcher/settings.bin");
            try
            {
                Inner = JObject.Parse(System.Text.Encoding.UTF8.GetString(
                    Convert.FromBase64String(File.ReadAllText(SettingsFile.ToString()))));
            }
            catch
            {
                Inner = new JObject();
            }
        }

        public static string SelectedServer
        {
            get => Inner.ContainsKey("server") ? (string) Inner["server"] : null;
            set
            {
                Inner["server"] = value;
                SaveSettings();
            }
        }

        public static string Username
        {
            get => Inner.ContainsKey("username") ? (string) Inner["username"] : null;
            set
            {
                Inner["username"] = value;
                SaveSettings();
            }
        }

        public static string Token
        {
            get => Inner.ContainsKey("token") ? (string) Inner["token"] : null;
            set
            {
                Inner["token"] = value;
                SaveSettings();
            }
        }        

        public static bool OpenConsoleOnPlay
        {
            get => Inner.ContainsKey("console_play") && (bool) Inner["console_play"];
            set
            {
                Inner["console_play"] = value;
                SaveSettings();
            }
        }

        public static string GetId()
        {
            if (!Inner.ContainsKey("id"))
            {
                Inner["id"] = Guid.NewGuid().ToString();
                SaveSettings();
            }
            return Inner["id"].ToString();
        }

        public static DateTime GetFirstLaunch(string server)
        {
            try
            {
                if (!Inner.ContainsKey("first_launch_"+server))
                {
                    Inner["first_launch_"+server] = DateTime.Now.ToBinary();
                    return DateTime.MinValue;
                }
                return DateTime.FromBinary(Inner["first_launch_"+server].Value<long>());
            }
            catch
            {
                Inner["first_launch_"+server] = ""+DateTime.Now.ToBinary();
                return DateTime.MinValue;
            }
        }

        public static DateTime GetLastVisit()
        {
            try
            {
                if (!Inner.ContainsKey("last_visit"))
                {
                    Inner["last_visit"] = DateTime.Now.ToBinary();
                    return DateTime.Now;
                }

                return DateTime.FromBinary(Inner["last_visit"].Value<long>());
            }
            catch
            {
                Inner["last_visit"] = ""+DateTime.Now.ToBinary();
                return DateTime.Now;
            }
        }
        public static void UpdateVisitValue()
        {
            Inner["last_visit"] = ""+DateTime.Now.ToBinary();
            SaveSettings();
        }

        public static int GetRunningRam(ServerInfo server)
        {
            var ram = GetSelectedRam(DataProvider.SelectedServer);
            if (ram<=300)
                ram = GetRamOptimal(DataProvider.SelectedServer);
            return ram;
        }

        public static int GetSelectedRam(ServerInfo server)
        {
            try
            {
                if (Inner.ContainsKey(server.ClientProfile.GetTitle()))
                    return (int) Inner[server.ClientProfile.GetTitle()];
            }
            catch
            {
            }
            return 0;
        }

        public static void SetSelectedRam(string server, int ram)
        {
            Inner[server] = ram;
            SaveSettings();
        }

        public static int GetRamOptimal(ServerInfo server)
        {
            int ramMin, ramRec;
            try
            {
                ramMin = (int) server.ClientProfile.ramMin.GetValue();
                ramRec = (int) server.ClientProfile.ramRec.GetValue();
            }
            catch
            {
                ramMin = 1024;
                ramRec = 2048;
            }

            try
            {
                var ramUserFree = HardwareInfo.GetRamFreeMb();
                ramUserFree = Math.Min(ramUserFree - 800, ramUserFree * 75 / 100);

                int ramUserOptimal;
                if (ramUserFree >= ramRec)
                    ramUserOptimal = ramRec;
                else if (ramUserFree >= ramMin)
                    ramUserOptimal = ramUserFree;
                else
                    ramUserOptimal = ramMin;
                return ramUserOptimal;
            }
            catch
            {
                return (ramRec + ramMin) / 2;
            }
        }

        public static bool HasReportFlag(string flag)
        {
            return Inner["reports"] is JArray array && array.Contains(flag);
        }

        public static void SetReportFlag(string flag)
        {
            if (!(Inner["reports"] is JArray array))
                Inner["reports"] = array = new JArray();
            array.Add(flag);
        }

        public static JToken InnerClone()
        {
            return Inner.DeepClone();
        }

        private static void SaveSettings()
        {
            SettingsFile.Directory.Create();
            File.WriteAllText(SettingsFile.ToString(),
                Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(Inner.ToString())));
        }

        public static void TryAuthWithSettings()
        {
            new Thread(() =>
            {
                try
                {
                    if (Inner.ContainsKey("token"))
                    {
                        DataProvider.AuthResult = new AuthTokenRequest(Inner["username"].ToString(), Inner["token"].ToString()).SendRequest();
                        MainWindow.Instance.Dispatcher.Invoke(new Action(delegate
                        {
                            MainWindow.Instance.ToggleLogin(false);
                        }));
                    }
                }
                catch
                {
                    Inner.Remove("token");
                    SaveSettings();
                }
            })
            {
                IsBackground = true
            }.Start();
        }
    }
}