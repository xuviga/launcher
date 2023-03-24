using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using iMine.Launcher.Client;
using iMine.Launcher.FileVisitor;
using iMine.Launcher.Request;
using iMine.Launcher.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace iMine.Launcher
{
    public static class ClientLauncher
    {
        private static DateTime clientLaunchTime;

        public static void GoGame(ServerInfo server, Params paramz)
        {
            FileWatchDoge.Watch(server.ClientProfile.GetDir());
            var profile = server.ClientProfile;
            clientLaunchTime = DateTime.MinValue;
            var args = new StringBuilder();

            //Добавляем аргументы для жава-машины
            var userRam = Settings.GetRunningRam(DataProvider.SelectedServer);
            args.Append($"-Xms{userRam-50}M ");
            args.Append($"-Xmx{userRam}M ");
            args.Append($"-Duser.dir=\"{paramz.clientDir}\" ");
            args.Append("-Djava.library.path=\"natives\" ");
            args.Append("-XX:+DisableAttachMechanism ");
            args.Append($"-DlauncherAddress=\"{Config.ServerIp}:{Config.ServerPort}\" ");
            foreach (var str in profile.GetJvmArgs())
                args.Append(str).Append(' ');

            //Подключаем жарки
            args.Append("-cp \"");
            var clientDir = paramz.clientDir;
            var clientLen = clientDir.FullName.Length + 1;
            var classpath = new List<string>();

            foreach (var path in profile.GetClassPath())
            {
                var pathDir = clientDir.ResolveDirectory(path);
                if (pathDir.Exists)
                {
                    var range = ResolveClassPathDir(clientLen, pathDir);
                    classpath.AddRange(range);
                }
                else
                    classpath.Add(path);
            }

            classpath.AddRange(ResolveLibrariesDir(Config.WorkingDir.FullName.Length+1, paramz.librariesDir));

            foreach (var str in classpath)
                args.Append(str).Append(';');

            //Задаем с какого класса запускается майнкрафт
            args.Append("\" ").Append(profile.GetMainClass()).Append(' ');

            //Добавляем аргументы уже самого майнкрафта
            foreach (var str in profile.GetClientArgs())
                args.Append(str).Append(' ');

            var logArgs = new StringBuilder(args.ToString());

            var clientArgs = new List<string>();
            var clientLogArgs = new List<string>();

            CompilateClientArgs(clientArgs, clientLogArgs, profile, paramz);

            foreach (var str in clientArgs)
                args.Append(str).Append(' ');

            foreach (var str in clientLogArgs)
                logArgs.Append(str).Append(' ');

            if (DataProvider.GameProcess != null && !DataProvider.GameProcess.HasExited)
            {
                App.WriteLog("Игра уже запущена. Не даем запустить игру второй раз");
                return;
            }

            var javaPathStr = UserHasHisJava() ? "java.exe" :
                '"'+Config.WorkingDir.ResolveFile("jre-8u131-win" + HardwareInfo.GetBits() + "/bin/java.exe").FullName+'"';

            App.WriteLog("Запускаем игру\n" + javaPathStr + " " + logArgs);

            var startInfo = new ProcessStartInfo(javaPathStr, args.ToString())
            {
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                WorkingDirectory = clientDir.FullName
            };

            var process = DataProvider.GameProcess = Process.Start(startInfo);
            if (process == null)
            {
                GoogleAnalytics.Exception("process_failed",null,"",true);
                throw new Exception("Не удалось запустить процесс");
            }
            clientLaunchTime = DateTime.Now;

            var logs = process.StandardOutput;
            var errors = process.StandardError;

            logs.ReadLine();
            var memoryErrorMessage = logs.ReadLine();
            if (memoryErrorMessage.ToLower().StartsWith("could not reserve enough space"))
            {
                App.WriteLog("Невозможно выделить достаточно памяти для игры\nПопробуй выделить меньше памяти в настройках");
                MessageBox.Show("Невозможно выделить достаточно памяти для игры\nПопробуй выделить меньше памяти в настройках");
                return;
            }
            new SeedRequest(Settings.Username, Settings.Token, memoryErrorMessage, logs.ReadLine()).SendRequest();

            new Thread(() =>
            {
                while (!logs.EndOfStream)
                    App.WriteLog(logs.ReadLine());
            })
            {
                IsBackground = true
            }.Start();

            new Thread(() =>
            {
                while (!errors.EndOfStream)
                    App.WriteLog(errors.ReadLine());
            })
            {
                IsBackground = true
            }.Start();

            process.Exited += (o, e) =>
            {
                if (clientLaunchTime == DateTime.MinValue)
                    return;
                var seconds = DateTime.Now.Subtract(clientLaunchTime).TotalSeconds;
                if (seconds<30)
                    if (!TryFindAReason(server))
                        GoogleAnalytics.Exception("client_closed",null,seconds.ToString(CultureInfo.InvariantCulture),true);
                GoogleAnalytics.ScreenView("main");
                App.WriteLog("Игра закрыта");
            };

            GoogleAnalytics.Event("launch","play",profile.GetTitle());
            GoogleAnalytics.ScreenView("playing");
        }

        public static void CloseGame()
        {
            clientLaunchTime = DateTime.MinValue;
            App.WriteLog("Игра принудительно остановлена");
            DataProvider.GameProcess.Kill();
        }

        public static bool UserHasHisJava()
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = "java.exe",
                    Arguments = " -version",
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                var process = Process.Start(startInfo);
                var versionLine = process.StandardError.ReadLine();
                var buildLine = process.StandardError.ReadLine();
                var bitLine = process.StandardError.ReadLine();

                var version = versionLine.Split(' ')[2].Replace("\"", "");
                var is64Bit = bitLine.ToLower().Contains("64-bit");

                try
                {
                    if (!is64Bit && !Settings.HasReportFlag("bits") && !UserHasOurJava())
                    {
                        GoogleAnalytics.Event("bits",HardwareInfo.GetBits()==64 ? "wrong_java" : "old_pc");
                    }
                }
                catch (Exception ignored)
                {
                }

                return is64Bit && version.StartsWith("1.8.");
            }
            catch
            {
                return false;
            }
        }

        public static bool UserHasOurJava()
        {
            try
            {
                var path = '"'+Config.WorkingDir.ResolveFile("jre-8u131-win" + HardwareInfo.GetBits() + "/bin/java.exe").FullName+'"';
                var startInfo = new ProcessStartInfo
                {
                    FileName = path,
                    Arguments = " -version",
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                var process = Process.Start(startInfo);
                var output = process.StandardError.ReadLine().Split(' ')[2].Replace("\"", "");
                return output.StartsWith("1.8.");
            }
            catch
            {
                return false;
            }
        }

        private static List<string> ResolveLibrariesDir(int rootLen, DirectoryInfo directoryInfo)
        {
            var result = new List<string> {"../"+directoryInfo.FullName.Substring(rootLen) + "/*"};
            foreach (var inner in directoryInfo.GetDirectories())
                result.AddRange(ResolveLibrariesDir(rootLen, inner));
            return result;
        }

        private static List<string> ResolveClassPathDir(int rootLen, DirectoryInfo directoryInfo)
        {
            var result = new List<string> {directoryInfo.FullName.Substring(rootLen) + "/*"};
            foreach (var inner in directoryInfo.GetDirectories())
                result.AddRange(ResolveClassPathDir(rootLen, inner));
            return result;
        }

        private static void CompilateClientArgs(List<string> args, List<string> logArgs, ClientProfile profile, Params paramz)
        {
            var playerProfile = paramz.pp;
            // Add version-dependent args
            var version = profile.GetVersion();
            AddArgs(args, logArgs, "--username", playerProfile.username);
            if (version.CompareTo(McVersion.Mc1_7_2) >= 0)
            {
                AddArgs(args, logArgs, "--uuid", ToHash(playerProfile.uuid));
                AddArgs(args, logArgs, "--accessToken", paramz.accessToken,"*******");

                if (version.CompareTo(McVersion.Mc1_7_10) >= 0)
                {
                    // Add user properties
                    AddArgs(args, logArgs, "--userType", "mojang");
                    /*var properties = new JObject();
                    if (playerProfile.skin != null)
                    {
                        properties.Add("skinURL", new JArray(playerProfile.skin.url));
                        properties.Add("skinDigest", new JArray(ToHex(playerProfile.skin.digest)));
                    }
                    if (playerProfile.cloak != null)
                    {
                        properties.Add("cloakURL", new JArray(playerProfile.cloak.url));
                        properties.Add("cloakDigest", new JArray(ToHex(playerProfile.cloak.digest)));
                    }
                    AddArgs(args, logArgs, "--userProperties", properties.ToString(Formatting.None).Replace("\"","\\\""));*/
                    //args.Add("--userProperties "+properties.ToString(Formatting.None));
                    //logArgs.Add("--userProperties "+properties.ToString(Formatting.None));

                    AddArgs(args, logArgs, "--userProperties", "{}");

                    // Add asset index
                    AddArgs(args, logArgs, "--assetIndex", profile.GetAssetIndex());
                }
            }
            else
            {
                AddArgs(args, logArgs, "--session", paramz.accessToken);
            }

            // Add version and dirs args
            AddArgs(args, logArgs, "--version", profile.GetVersion().Name);
            AddArgs(args, logArgs, "--gameDir", paramz.clientDir.ToString());
            AddArgs(args, logArgs, "--assetsDir", paramz.assetDir.ToString());
            AddArgs(args, logArgs, "--resourcePackDir", paramz.clientDir.ResolveDirectory("resourcepacks").ToString());
            if (version.CompareTo(McVersion.Mc1_9_4) >= 0)
            {
                // Just to show it in debug screen
                AddArgs(args, logArgs, "--versionType", "Launcher v" + Assembly.GetEntryAssembly().GetName().Version);
            }

            // Add server args
            if (paramz.autoEnter)
            {
                AddArgs(args, logArgs, "--server", profile.GetServerAddress());
                AddArgs(args, logArgs, "--port", profile.GetServerPort().ToString());
            }

            // Add window size args
            if (paramz.fullScreen)
            {
                AddArgs(args, logArgs, "--fullscreen","true");
            }

            if (paramz.width > 0 && paramz.height > 0)
            {
                AddArgs(args, logArgs, "--width", paramz.width.ToString());
                AddArgs(args, logArgs, "--height", paramz.height.ToString());
            }
        }

        private static void AddArgs(ICollection<string> args, ICollection<string> logArgs,  string key, string value, string hiddenValue=null)
        {
            args.Add(key);
            logArgs.Add(key);
            args.Add('"'+value+'"');
            logArgs.Add('"'+(hiddenValue ?? value)+'"');
        }

        private static bool TryFindAReason(ServerInfo server)
        {
            string message = null;
            try
            {
                var stacktrace = App.GetConsoleText();
                stacktrace = stacktrace.Substring(Math.Max(0, stacktrace.Length - 16384));
                if (stacktrace.Contains("Picked up _JAVA_OPTIONS"))
                {
                    message = $"В настройках системы стоит указан вредный параметр _JAVA_OPTIONS.\nПерейдите в меню \"Пуск\", введите \"Переменные среды\", нажмите верхнюю опцию.\nЗатем в обеих переменных System и User удалите значения для _JAVA_OPTIONS из ваших переменных среды.";
                }
                else if (stacktrace.Contains("Could not reserve enough space for"))
                {
                    message = $"Похоже, компьютер не смог выделить {Settings.GetRunningRam(server)}МБ RAM.\nПопробуй выделить меньше памяти в настройках.";
                }
                else if (stacktrace.Contains("java.lang.OutOfMemoryError"))
                {
                    message = $"Похоже, игре не хватает выделенных {Settings.GetRunningRam(server)}МБ RAM.\nПопробуй выделить больше памяти в настройках.";
                }
            }
            catch
            {
            }

            if (message == null)
                return false;
            MessageBox.Show(message);
            return true;

        }

        private static string ToHash(Guid uuid)
        {
            return uuid.ToString().Replace("-", "");
        }

        public static string ToHex(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "");
        }

        public static void OpenWebPage(string url)
        {
            try
            {
                Process.Start("cmd", "/C start "+url);
            }
            catch (Exception e)
            {
                App.WriteException(e, "Не удалось открыть ссылку: "+url);
                GoogleAnalytics.Exception("open_url",null,url,false);
            }
        }
    }
}