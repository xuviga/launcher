using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using iMine.Launcher.Utils;
using iMine.Launcher.Windows;

namespace iMine.Launcher
{
    public partial class App
    {
        private static readonly StringWriter TextWriter = new StringWriter(new StringBuilder(50000));

        public static bool IsShuttingDown;
        private static Mutex mutex;

        private void OnStart(object sender, StartupEventArgs ev)
        {
            mutex = new Mutex(true, "iMineLauncher", out var ok);

            if (!ok)
            {
                MessageBox.Show("Лаунчер iMine уже запущен");
                Environment.Exit(0);
                return;
            }

            Config.Init(ev.Args[0], int.Parse(ev.Args[1]), ev.Args.Length == 2 ? "" : ev.Args[2]);
            GoogleAnalytics.Init();
            var mainWindow = new MainWindow();
            var consoleWindow = new ConsoleWindow();
            consoleWindow.Left = mainWindow.Left + (mainWindow.Width - consoleWindow.Width) / 2;
            consoleWindow.Top = mainWindow.Top + (mainWindow.Height - consoleWindow.Height) / 2;

            StreamWriter fileWriter = null;
            try
            {
                fileWriter = new StreamWriter(new FileStream("iMineLauncher.log", FileMode.Create));
            }
            catch
            {
            }

            var consoleWriter = Console.Out;
            var textBoxWriter = new TextBoxWriter(consoleWindow.ConsoleBox);
            var superWriter = new SuperTextWriter(new[] {fileWriter, textBoxWriter, consoleWriter, TextWriter});
            Console.SetOut(superWriter);
            Console.SetError(superWriter);

            DispatcherUnhandledException += OnException;
            Exit += (s, e) => { IsShuttingDown = true; };

            try
            {
                WriteLog("iMine Launcher v." + GetVersion());
            }
            catch
            {
            }

            ImageCache.Init();

            new Thread(() =>
            {
                try
                {
                    DataProvider.DoEveryfin();
                    Settings.TryAuthWithSettings();
                    Current.Dispatcher.Invoke(new Action(delegate
                    {
                        WriteLog("Создаем интерфейс...");
                        try
                        {
                            DataProvider.SelectedServer = DataProvider.Servers.First(it => it.Key == Settings.SelectedServer).Value;
                        }
                        catch
                        {
                            DataProvider.SelectedServer = DataProvider.Servers.Values.First();
                        }

                        WriteLog("Создаем элементы интерфейса...");
                        mainWindow.ProcessServers(DataProvider.Servers.Values, DataProvider.Links);
                        mainWindow.UpdateDisplay();
                        WriteLog("Открываем главное окно...");
                        GoogleAnalytics.ScreenView("main");
                        mainWindow.Show();
                        LoadingWindow.Instance?.Close();
                        mainWindow.Focus();
                    }));
                }
                catch (Exception e)
                {
                    WriteImportantException(e);
                }
            }).Start();
        }

        private static void OnException(object sender, DispatcherUnhandledExceptionEventArgs ev)
        {
            ev.Handled = true;
            WriteImportantException(ev.Exception);
        }

        public static string GetConsoleText()
        {
            return TextWriter.ToString();
        }

        public static void WriteLog(object value)
        {
            Console.WriteLine(value.ToString());
        }

        public static void WriteException(Exception e, string info = "")
        {
            Console.WriteLine(info);
            Console.WriteLine(e);
            if (Launcher.Windows.MainWindow.Instance == null)
            {
                GoogleAnalytics.EndSession("exception");
                IsShuttingDown = true;
            }

            if (ConsoleWindow.Instance != null)
            {
                Current.Dispatcher.Invoke(new Action(delegate { ConsoleWindow.Instance.Show(); }));
            }
        }

        public static void WriteImportantException(Exception e, string info = "")
        {
            Console.WriteLine(info);
            Console.WriteLine(e);
            if (Launcher.Windows.MainWindow.Instance == null)
            {
                GoogleAnalytics.EndSession("exception");
                IsShuttingDown = true;
            }

            if (ConsoleWindow.Instance != null)
            {
                Current.Dispatcher.Invoke(new Action(delegate
                {
                    ConsoleWindow.Instance.Show();
                    BugreportWindow.Show(false);
                }));
            }
        }

        public static string GetVersion()
        {
            return System.Diagnostics.FileVersionInfo.GetVersionInfo(ResourceAssembly.Location).ProductVersion;
        }
    }
}