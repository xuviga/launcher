using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using iMine.Launcher.Client;
using iMine.Launcher.Utils;
using System.Diagnostics;

namespace iMine.Launcher.Windows
{
    public partial class SettingsWindow
    {
        private ServerInfo server;
        public SettingsWindow(ServerInfo server)
        {
            this.server = server;
            InitializeComponent();
            
            Background = new ImageBrush
            {
                ImageSource = ImageCache.BlankDialog
            };

            CloseButton.Background = new ImageBrush
            {
                ImageSource = ImageCache.CloseButtonOnly
            };
            CloseButton.MouseDown += (s, e) => { Close(); };

            SecondaryWindowController.HookWindow(this);
            PostInitializeComponent(server);

            Owner = MainWindow.Instance;
            Left = Owner.Left + (Owner.Width - Width) / 2;
            Top = Owner.Top + (Owner.Height - Height) / 2;
        }

        private void PostInitializeComponent(ServerInfo server)
        {
            var ratio = SystemParameters.PrimaryScreenWidth / 1920 * 1.3;
            Width *= ratio;
            Height *= ratio;

            ApplyButton.Background = new ImageBrush
            {
                ImageSource = ImageCache.GoldButton
            };
            ApplyButton.Click += (s, e) => { Close(); };

            CheckerOpenConsoleOnPlay.IsChecked = Settings.OpenConsoleOnPlay;

            int ramMin, ramRec, systemRamRec;
            try
            {
                ramMin = (int)server.ClientProfile.ramMin.GetValue();
                systemRamRec = (int)server.ClientProfile.systemRamRec.GetValue();
                ramRec = (int)server.ClientProfile.ramRec.GetValue();
            }
            catch
            {
                ramMin = 1024;
                ramRec = 2048;
                systemRamRec = 4096;
            }
            var ramUser = HardwareInfo.GetRamSizeMb();
            var ramFree = HardwareInfo.GetRamFreeMb();
            if (ramUser < 500)
                ramUser = systemRamRec;

            MemorySlider.ValueChanged += (s, e) =>
            {
                MemoryWarning.Document.Blocks.Clear();

                var value = MemorySlider.Value;
                if ((int) value <= 300)
                {
                    MemoryValue.Content = "Автоматически";
                    return;
                }

                MemoryValue.Content = (int) value + "МБ";
                if (value > ramRec)
                {
                    var paragraph = new Paragraph();
                    paragraph.Inlines.Add(new Run($"Можно уменьшить выделенную память. Выделение памяти свыше {ramRec}МБ не дает заметного прироста.")
                    {
                        Foreground = Brushes.Green
                    });
                    MemoryWarning.Document.Blocks.Add(paragraph);
                }

                if (value < ramMin)
                {
                    var paragraph = new Paragraph();
                    paragraph.Inlines.Add(new Run("Выделено недостаточно памяти! Вероятны перебои в работе игры!")
                    {
                        Foreground = Brushes.Red
                    });
                    MemoryWarning.Document.Blocks.Add(paragraph);
                }
                else
                {
                    if (ramUser < ramFree)
                    {
                        var paragraph = new Paragraph();
                        paragraph.Inlines.Add(new Run($"В данный момент свободно лишь {ramFree}МБ. Возможно, стоит выделить меньше памяти, или закрыть прочие программы."));
                        paragraph.Foreground = Brushes.DarkOrange;
                        MemoryWarning.Document.Blocks.Add(paragraph);
                    }
                    if (ramUser < systemRamRec)
                    {
                        var paragraph = new Paragraph();
                        paragraph.Inlines.Add(new Run("На компьютере установлено умеренно памяти! Если возникнут проблемы, рекомендуется закрыть прочие программы и выделять памяти игре не больше необходимой."));
                        paragraph.Foreground = Brushes.Black;
                        MemoryWarning.Document.Blocks.Add(paragraph);
                    }
                }
            };
            MemorySlider.Ticks = MemoryTickBar.Ticks = MemoryTickBar2.Ticks = new DoubleCollection(new List<double> { ramMin, ramRec, ramUser });
            MemorySlider.Maximum = MemoryTickBar.Maximum = MemoryTickBar2.Maximum = ramRec + 1024;
            var userRam = Settings.GetSelectedRam(DataProvider.SelectedServer);
            MemorySlider.Value = userRam;
            RamPointMin.Offset = ramMin / MemorySlider.Maximum;
            RamPointMid.Offset = Math.Min(ramMin + 300,(ramRec+ramMin) / 2) / MemorySlider.Maximum;
            RamPointRec.Offset = ramRec / MemorySlider.Maximum;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            MainWindow.Instance.Focus();
            GoogleAnalytics.ScreenView("main");
            var value = (int) MemorySlider.Value;
            Settings.SetSelectedRam(DataProvider.SelectedServer.ClientProfile.GetTitle(), value <= 300 ? 0 : value);
        }

        private void ToggleConsole(object sender, RoutedEventArgs e)
        {
            Settings.OpenConsoleOnPlay = ((CheckBox) sender).IsChecked ?? false;
        }

        private void CloseButtonEnter(object sender, MouseEventArgs e)
        {
            ((StackPanel)sender).Background = new ImageBrush
            {
                ImageSource = ImageCache.CloseButtonOnlyGlowing
            };
        }

        private void CloseButtonLeave(object sender, MouseEventArgs e)
        {
            ((StackPanel)sender).Background = new ImageBrush
            {
                ImageSource = ImageCache.CloseButtonOnly
            };
        }

        private void GoldButtonEnter(object sender, MouseEventArgs e)
        {
            ((Button) sender).Background = new ImageBrush
            {
                ImageSource = ImageCache.GoldButtonHover
            };
        }

        private void GoldButtonLeave(object sender, MouseEventArgs e)
        {
            ((Button) sender).Background = new ImageBrush
            {
                ImageSource = ImageCache.GoldButton
            };
        }

        private void ResetMemory(object sender, RoutedEventArgs e)
        {
            Settings.SetSelectedRam(DataProvider.SelectedServer.ClientProfile.GetTitle(), 0);
            MemorySlider.Value = 0;
        }

        private void OpenLogs(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(Config.WorkingDir.ResolveDirectory(
                    server.ClientProfile.GetDir()).ResolveDirectory("logs").FullName);
            }
            catch { }
        }

        private void OpenScreenshots(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(Config.WorkingDir.ResolveDirectory(
                    server.ClientProfile.GetDir()).ResolveDirectory("screenshots").FullName);
            }
            catch { }
        }
    }
}
