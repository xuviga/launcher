using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using iMine.Launcher.Client;
using iMine.Launcher.Request;
using iMine.Launcher.Request.Update;
using iMine.Launcher.Utils;
using System.Windows;
using iMine.Launcher.Windows;

namespace iMine.Launcher
{
    public static class Config
    {
        public static readonly Uri OurWebsite = new Uri("ws://121.0.0.0:7240/api");
        private static readonly MD5 Md5 = MD5.Create();

        public static DirectoryInfo WorkingDir { get; private set; }
        public static string ServerIp { get; private set; }
        public static int ServerPort { get; private set; }
        public static string DownloaderPath { get; private set; }

        public static void Init(string serverIp, int serverPort, string downloaderPath)
        {
            ServerIp = serverIp;
            ServerPort = serverPort;
            DownloaderPath = downloaderPath;

            WorkingDir = new DirectoryInfo(Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData)).ResolveDirectory("iMine");
            WorkingDir.Create();
        }

        public static List<ClientProfile> ConnectLauncher()
        {
            new PingRequest().SendRequest();
            var data = new LauncherRequest().SendRequest();
            return data.ConvertAll(it => it.Obj);
        }

        public static void RunClient(ServerInfo serverInfo)
        {
            if (DataProvider.AuthResult == null)
            {
                App.WriteLog("Открываем окно авторизации вместо запуска игры");
                GoogleAnalytics.ScreenView("login_with_play");
                OpenNewLoginWindow(serverInfo);
                return;
            }
            App.WriteLog("Аккаунт подтвержден. Запускаем игру...");

            var clientProfile = serverInfo.ClientProfile;
            try
            {
                var paramz = new Params(new byte[0],
                    WorkingDir.ResolveDirectory(clientProfile.GetAssetDir()),
                    WorkingDir.ResolveDirectory(clientProfile.GetLibrariesDir()),
                    WorkingDir.ResolveDirectory(clientProfile.GetDir()),
                    DataProvider.AuthResult.PlayerProfile,
                    DataProvider.AuthResult.AccessToken,
                    false, false, 3000, 0, 0);
                ClientLauncher.GoGame(serverInfo, paramz);
            }
            catch (Exception e)
            {
                App.WriteImportantException(e,$"Не удалось запустить игру\n{serverInfo}\n{clientProfile}\n{DataProvider.AuthResult.PlayerProfile}");
                GoogleAnalytics.Exception("run_game",null,$"{serverInfo}|{clientProfile}|{DataProvider.AuthResult.PlayerProfile}",true);
            }
        }

        public static void DownloadClientAndRun(ServerInfo serverInfo)
        {
            if (DataProvider.IsGameInProgress())
            {
                App.WriteLog("Попытка запустить игру с запущенной игрой или закачкой. Отменяем.");
                return;
            }

            App.WriteLog("Нажата кнопка играть");
            if (DataProvider.AuthResult == null)
            {
                GoogleAnalytics.ScreenView("login_with_download");
                OpenNewLoginWindow(serverInfo);
            }

            var downloadWindow = new DownloadingWindow();

            DataProvider.DownloadThread = new Thread(() =>
            {
                try
                {
                    var clientProfile = serverInfo.ClientProfile;
                    GoogleAnalytics.Event("launch","check",clientProfile.GetTitle());

                    if (!ClientLauncher.UserHasHisJava())
                    {
                        do
                        {
                            var text = "Получаем метаданные jre-8u131-win" + HardwareInfo.GetBits() + "...";
                            Application.Current.Dispatcher.Invoke(new Action(delegate { downloadWindow.DownloadDesc.Text = text; }));
                            App.WriteLog(text);
                            downloadWindow.Request = new UpdateRequest("jre-8u131-win" + HardwareInfo.GetBits(), WorkingDir.ResolveDirectory("jre-8u131-win" + HardwareInfo.GetBits()), null, true);
                        }
                        while (downloadWindow.Request.SendRequest().Obj.anyMismatches);
                    }

                    do
                    {
                        var text = $"Получаем метаданные {clientProfile.GetAssetDir()}...";
                        Application.Current.Dispatcher.Invoke(new Action(delegate { downloadWindow.DownloadDesc.Text = text; }));
                        App.WriteLog(text);
                        downloadWindow.Request = new UpdateRequest(clientProfile.GetAssetDir(), WorkingDir.ResolveDirectory(clientProfile.GetAssetDir()),
                            ClientProfile.AssetMatcher, true);
                    }
                    while (downloadWindow.Request.SendRequest().Obj.anyMismatches);

                    do
                    {
                        var text = $"Получаем библиотеки {clientProfile.GetLibrariesDir()}...";
                        Application.Current.Dispatcher.Invoke(new Action(delegate { downloadWindow.DownloadDesc.Text = text; }));
                        App.WriteLog(text);
                        downloadWindow.Request = new UpdateRequest(clientProfile.GetLibrariesDir(), WorkingDir.ResolveDirectory(clientProfile.GetLibrariesDir()),
                            ClientProfile.LibrariesMatcher, true);
                    }
                    while (downloadWindow.Request.SendRequest().Obj.anyMismatches);

                    do
                    {
                        var text = $"Получаем метаданные {clientProfile.GetDir()}...";
                        Application.Current.Dispatcher.Invoke(new Action(delegate { downloadWindow.DownloadDesc.Text = text; }));
                        App.WriteLog(text);
                        downloadWindow.Request = new UpdateRequest(clientProfile.GetDir(), WorkingDir.ResolveDirectory(clientProfile.GetDir()),
                            clientProfile.GetClientUpdateMatcher(), true);
                    }
                    while (downloadWindow.Request.SendRequest().Obj.anyMismatches);

                    App.WriteLog($"Обновление клиента {serverInfo.ClientProfile.GetTitle()} завершено");
                    Application.Current.Dispatcher.Invoke(new Action(delegate
                    {
                        downloadWindow.ImOk = true;
                        downloadWindow.Close();
                    }));
                    RunClient(serverInfo);
                }
                catch (ThreadAbortException ignored)
                {
                }
                catch (Exception e)
                {
                    Application.Current.Dispatcher.Invoke(new Action(delegate
                    {
                        downloadWindow.Close();
                    }));
                    App.WriteLog("Произошел сбой подключения. Попробуй снова.");
                    MessageBox.Show("Произошел сбой подключения. Попробуй снова.");
                    App.WriteImportantException(e);
                    //GoogleAnalytics.Exception("download",null,"",true);
                }
            })
            {
                IsBackground = true
            };
            DataProvider.DownloadThread.Start();

            GoogleAnalytics.ScreenView("download");
            downloadWindow.Show();
        }

        public static void StopDownloadingClient()
        {
            App.WriteLog("Отменяем обновление игры");
            DataProvider.DownloadThread?.Abort();
        }

        private static void OpenNewLoginWindow(ServerInfo serverInfo)
        {
            Application.Current.Dispatcher.Invoke(new Action(delegate
            {
                foreach (Window window in Application.Current.Windows)
                {
                    if (window is LoginWindow || window is RegisterWindow)
                        return;
                }

                new LoginWindow
                {
                    WannaStartGameAfter = serverInfo
                }.Show();
            }));
        }

        public static byte[] ComputeHash(byte[] bytes)
        {
            return Md5.ComputeHash(bytes);
        }

        public static byte[] ComputeHash(Stream stream)
        {
            return Md5.ComputeHash(stream);
        }

        public static byte[] ComputeHash(string path)
        {
            using(var stream = new BufferedStream(File.OpenRead(path)))
            {
                var hash = Md5.ComputeHash(stream);
                return hash;
            }
        }
    }
}