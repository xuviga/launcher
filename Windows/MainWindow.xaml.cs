using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using iMine.Launcher.Client;
using iMine.Launcher.Utils;
using iMine.Launcher.Utils.Collections;
using Newtonsoft.Json.Linq;
using System.Windows.Shapes;
using System.Threading;

namespace iMine.Launcher.Windows
{
    public partial class MainWindow
    {
        public static MainWindow Instance { get; private set; }

        private int currentNew;
        private int currentChangeLog;
        public bool IsClosed { get; private set; }

        public MainWindow()
        {
            Instance = this;
            IsClosed = false;

            InitializeComponent();
            PostInitializeComponent();
            Focus();

            new Thread(() =>
            {
                for (;;)
                {
                    Thread.Sleep(2000);
                    Dispatcher.Invoke(new Action(ProcessCheckList));
                    if (Visibility == Visibility.Hidden)
                        break;
                }
            })
            {
                IsBackground = true
            }.Start();
        }

        public void PostInitializeComponent()
        {
            var ratio = SystemParameters.PrimaryScreenWidth / 1920 * 1.3;
            if (Math.Abs(SystemParameters.PrimaryScreenWidth / SystemParameters.PrimaryScreenHeight - 1.333) < 0.2)
                ratio *= 1.2;
            Width *= ratio;
            Height *= ratio;

            MainGrid.Background = new ImageBrush
            {
                ImageSource = ImageCache.MainBcg
            };

            MainSlider.Background = new ImageBrush
            {
                ImageSource = ImageCache.Nope
            };
            BottomSlider0.Background = new ImageBrush
            {
                ImageSource = ImageCache.Nope
            };

            BottomSlider1.Background = new ImageBrush
            {
                ImageSource = ImageCache.Nope
            };

            BottomSlider2.Background = new ImageBrush
            {
                ImageSource = ImageCache.Nope
            };

            Logo.Background = new ImageBrush
            {
                ImageSource = ImageCache.Logo
            };

            LoginButton.Background = ServerInfoButton.Background = new ImageBrush
            {
                ImageSource = ImageCache.GoldButton
            };

            RegisterButton.Background = new ImageBrush
            {
                ImageSource = ImageCache.DiamondButton
            };

            SettingsButton.Background = new ImageBrush
            {
                ImageSource = ImageCache.Cog
            };

            CloseButton.Background = new ImageBrush
            {
                ImageSource = ImageCache.CloseButton
            };

            PlayButtonBcg.Background = new ImageBrush
            {
                ImageSource = ImageCache.PlayButtonInactive
            };

            ModListPanel.Background = new ImageBrush
            {
                ImageSource = ImageCache.ModListPanel
            };

            BlackTint.Background = new ImageBrush
            {
                ImageSource = ImageCache.MainBcgTint
            };

            ConsoleButton.Background = new ImageBrush
            {
                ImageSource = ImageCache.Console
            };

            BugButton.Background = new ImageBrush
            {
                ImageSource = ImageCache.Bug
            };

            SliderOpacity.OpacityMask = BottomSliderOpacity1.OpacityMask = BottomSliderOpacity2.OpacityMask = BottomSliderOpacity3.OpacityMask =
                new ImageBrush
                {
                    ImageSource = ImageCache.TintMask
                };

            InputManager.Current.PreProcessInput += (sender, e) =>
            {
                if (e.StagingItem.Input is MouseButtonEventArgs
                    && ModPanel.Visibility == Visibility.Visible && !ModPanel.IsMouseOver)
                {
                    ModPanel.Visibility = Visibility.Hidden;
                }
            };
        }

        private void SettingsButtonClick(object sender, RoutedEventArgs e)
        {
            GoogleAnalytics.ScreenView("settings");
            new SettingsWindow(DataProvider.SelectedServer).Show();
        }

        private static void LinkButtonClick(object sender, RoutedEventArgs e)
        {
            ClientLauncher.OpenWebPage(((Button) sender).Tag.ToString());
        }

        private void LoginButtonClick(object sender, RoutedEventArgs e)
        {
            if (LoginButton.Content == "Выйти")
            {
                DataProvider.AuthResult = null;
                Settings.Token = null;
                ToggleLogin(true);
            }
            else
            {
                GoogleAnalytics.ScreenView("login");
                new LoginWindow().Show();
            }
        }

        private void RegisterButtonClick(object sender, RoutedEventArgs e)
        {
            GoogleAnalytics.ScreenView("register");
            new RegisterWindow().Show();
        }

        private void PrevSlideButtonClick(object sender, RoutedEventArgs e)
        {
            currentNew--;
            if (currentNew < 0)
                currentNew = DataProvider.SelectedServer.News.Count - 1;
            SetCurrentNews(currentNew);
        }

        private void NextSlideButtonClick(object sender, RoutedEventArgs e)
        {
            currentNew++;
            if (currentNew >= DataProvider.SelectedServer.News.Count)
                currentNew = 0;
            SetCurrentNews(currentNew);
        }

        private void ServerInfoTagClick(object sender, RoutedEventArgs e)
        {
            for (var i = 0; i < DataProvider.SelectedServer.News.Count; i++)
            {
                var news = DataProvider.SelectedServer.News[i];
                if (news.Title.Equals(DataProvider.SelectedServer.ClientProfile.GetTitle()))
                {
                    SetCurrentNews(i);
                    break;
                }
            }
        }

        private void PrevBottomSlideButtonClick(object sender, RoutedEventArgs e)
        {
            currentChangeLog--;
            if (currentChangeLog < 0)
                currentChangeLog = 0;
            SetCurrentChangeLog(currentChangeLog);
        }

        private void NextBottomSlideButtonClick(object sender, RoutedEventArgs e)
        {
            currentChangeLog++;
            if (currentChangeLog > DataProvider.SelectedServer.ChangeLogs.Count - 3)
                currentChangeLog--;
            SetCurrentChangeLog(currentChangeLog);
        }

        private void ServerButtonClick(object sender, RoutedEventArgs e)
        {
            var butt = (Button) sender;
            var serverName = butt.Tag.ToString();
            var server = DataProvider.Servers[serverName];
            if (server != null)
            {
                DataProvider.SelectedServer = server;
                UpdateDisplay();
                Settings.SelectedServer = server.ClientProfile.GetTitle();
            }

            foreach (var child in MainGrid.Children)
            {
                if (child is Button otherButt)
                    otherButt.Effect = null;
            }
            butt.Effect = new DropShadowEffect
            {
                BlurRadius = 8,
                Color = Color.FromArgb(255, 239, 212, 35),
                ShadowDepth = 0,
                Opacity = 1
            };
        }

        private void PlayButtonClick(object sender, RoutedEventArgs e)
        {
            Config.DownloadClientAndRun(DataProvider.SelectedServer);
            if (Settings.OpenConsoleOnPlay)
                ConsoleWindow.Instance.Show();
        }

        private void CloseButtonClick(object sender, MouseButtonEventArgs e)
        {
            if (e.GetPosition(CloseButton).X < 30)
                WindowState = WindowState.Minimized;
            else
                Close();
        }

        private void CloseButtonMove(object sender, MouseEventArgs e)
        {
            if (e.GetPosition(CloseButton).X < 30)
                ((ImageBrush)CloseButton.Background).ImageSource = ImageCache.MinimizeButtonGlowing;
            else
                ((ImageBrush)CloseButton.Background).ImageSource = ImageCache.CloseButtonGlowing;
        }

        private void CloseButtonLeave(object sender, MouseEventArgs e)
        {
            ((ImageBrush)CloseButton.Background).ImageSource = ImageCache.CloseButton;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            IsClosed = false;
            Hide();
            ConsoleWindow.Instance?.Hide();
            Settings.UpdateVisitValue();
            GoogleAnalytics.EndSession("main");
            App.IsShuttingDown = true;
        }

        private void DragMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
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

        private void DiamondButtonEnter(object sender, MouseEventArgs e)
        {
            ((Button) sender).Background = new ImageBrush
            {
                ImageSource = ImageCache.DiamondButtonHover
            };
        }

        private void DiamondButtonLeave(object sender, MouseEventArgs e)
        {
            ((Button) sender).Background = new ImageBrush
            {
                ImageSource = ImageCache.DiamondButton
            };
        }

        private void ModHelpMouseEnter(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void ModHelpMouseLeave(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void MainSliderTextMouseEnter(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Hand;
            MainSliderText.Background = new SolidColorBrush(Color.FromArgb(191, 68, 57, 54));
        }

        private void MainSliderTextMouseLeave(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
            MainSliderText.Background = new SolidColorBrush(Color.FromArgb(191, 58, 47, 44));
        }

        private void BottomSliderTextMouseEnter(object sender, MouseEventArgs e)
        {
            var grid = (Grid)sender;
            var tint = grid.FindName(grid.Name + "Tint");
            Mouse.OverrideCursor = Cursors.Hand;
            ((Rectangle)tint).Opacity = 0.1;
        }

        private void BottomSliderTextMouseLeave(object sender, MouseEventArgs e)
        {
            var grid = (Grid)sender;
            var tint = grid.FindName(grid.Name + "Tint");
            Mouse.OverrideCursor = Cursors.Arrow;
            ((Rectangle)tint).Opacity = 0;
        }

        private void MainSliderMouseEnter(object sender, MouseEventArgs e)
        {
            PrevSlideButton.Opacity = 0.7;
            NextSlideButton.Opacity = 0.7;
        }

        private void MainSliderMouseLeave(object sender, MouseEventArgs e)
        {
            PrevSlideButton.Opacity = 0.5;
            NextSlideButton.Opacity = 0.5;
        }

        private void PrevSlideButtonMouseEnter(object sender, MouseEventArgs e)
        {
            PrevSlideButton.Opacity = 0.9;
        }

        private void PrevSlideButtonMouseLeave(object sender, MouseEventArgs e)
        {
            PrevSlideButton.Opacity = 0.7;
        }

        private void NextSlideButtonMouseEnter(object sender, MouseEventArgs e)
        {
            NextSlideButton.Opacity = 0.9;
        }

        private void NextSlideButtonMouseLeave(object sender, MouseEventArgs e)
        {
            NextSlideButton.Opacity = 0.7;
        }

        private void PrevButtonMouseEnter(object sender, MouseEventArgs e)
        {
            PrevBottomButton.Opacity = 1;
        }

        private void NextButtonMouseEnter(object sender, MouseEventArgs e)
        {
            NextBottomButton.Opacity = 1;
        }

        private void PrevButtonMouseLeave(object sender, MouseEventArgs e)
        {
            PrevBottomButton.Opacity = 0.5;
        }

        private void NextButtonMouseLeave(object sender, MouseEventArgs e)
        {
            NextBottomButton.Opacity = 0.5;
        }

        private void ConsoleButtonClick(object sender, RoutedEventArgs e)
        {
            ConsoleWindow.Instance?.Show();
        }

        private void BugButtonClick(object sender, RoutedEventArgs e)
        {
            BugreportWindow.Show(true);
        }

        private void ConsoleButtonEnter(object sender, MouseEventArgs e)
        {
            ((Button)sender).Opacity = 0.8;
        }

        private void ConsoleButtonLeave(object sender, MouseEventArgs e)
        {
            ((Button)sender).Opacity = 0.5;
        }

        public void ModHelpButtonClick(object sender, MouseEventArgs e)
        {
            new Thread(() =>
            {
                Thread.Sleep(200);
                Dispatcher.Invoke(new Action(delegate
                {
                    ModPanel.Visibility = Visibility.Visible;
                    var mouse = Mouse.GetPosition(MainGrid);
                    ModPanel.Margin = new Thickness(mouse.X - 15 - ModPanel.Width, mouse.Y - 5, 0, 0);
                    ModPanelBox.Document.Blocks.Clear();
                    var line = (Inline)sender;
                    var json = (JObject)((Paragraph)line.Parent).Tag;
                    ModPanelBox.AppendText((string)json["desc"]);
                    ModPanelLink.NavigateUri = new Uri((string)json["link"]);
                    ModPanelBox.UpdateLayout();
                    ModPanel.Height = ModPanelBox.ExtentHeight + 20;
                }));
            }).Start();
        }

        private void ModPanelLinkClicked(object sender, EventArgs e)
        {
            ClientLauncher.OpenWebPage(ModPanelLink.NavigateUri.ToString());
        }

        private void MouseEnterServerButton(object sender, MouseEventArgs e)
        {
            var button = (Button)sender;
            ((ImageBrush)button.Background).ImageSource = ImageCache.ServerButtonActive;
            var block = (TextBlock) button.Content;
            var color = Color.FromArgb(255, 229, 179, 79);
            block.Foreground = new SolidColorBrush(color);
            block.Effect = new DropShadowEffect
            {
                BlurRadius = 15,
                Color = color,
                ShadowDepth = 0
            };
        }

        private void MouseLeaveServerButton(object sender, MouseEventArgs e)
        {
            var button = (Button)sender;
            ((ImageBrush)button.Background).ImageSource = ImageCache.ServerButtonInactive;
            var block = (TextBlock) button.Content;
            var color = Color.FromArgb(181, 159, 140, 123);
            block.Foreground = new SolidColorBrush(color);
            block.Effect = null;
        }

        private void MouseEnterPlayButton(object sender, MouseEventArgs e)
        {
            ((ImageBrush)PlayButtonBcg.Background).ImageSource = ImageCache.PlayButtonActive;
        }

        private void MouseLeavePlayButton(object sender, MouseEventArgs e)
        {
            ((ImageBrush)PlayButtonBcg.Background).ImageSource = ImageCache.PlayButtonInactive;
        }

        private void MainSliderTextClick(object sender, RoutedEventArgs e)
        {
            ClientLauncher.OpenWebPage(new Uri(DataProvider.SelectedServer.News[currentNew].ClickUrl).ToString());
        }

        private void BottomSliderTextClick(object sender, MouseButtonEventArgs e)
        {
            var name = ((Grid)sender).Name;
            var path = DataProvider.SelectedServer.ChangeLogs[currentChangeLog + int.Parse(name.Substring(name.Length - 1))].ClickUrl;
            ClientLauncher.OpenWebPage(new Uri(path).ToString());
        }

        public void ToggleLogin(bool wut)
        {
            if (wut)
            {
                RegisterButton.Visibility = Visibility.Visible;
                LoginLabel.Visibility = Visibility.Hidden;
                LoginLabel.Text = "";
                LoginButton.Content = "Логин";
            }
            else
            {
                RegisterButton.Visibility = Visibility.Hidden;
                LoginLabel.Text = DataProvider.AuthResult.PlayerProfile.username;
                LoginLabel.Visibility = Visibility.Visible;
                LoginButton.Content = "Выйти";
            }
        }

        private Run GetSign(int warnLevel)
        {
            switch (warnLevel)
            {
                case 2:
                    return new Run("\u26A0") {Foreground = Brushes.Red};
                case 1:
                    return new Run("\u26A0") {Foreground = Brushes.Gold};
                default:
                    return new Run("\u2713") {Foreground = Brushes.LimeGreen};
            }
        }

        private void ShowCheckList()
        {
            ModList.Height = 255;
            CheckListBorder.Height = 90;
        }

        private void MininizeCheckList()
        {
            ModList.Height = 315;
            CheckListBorder.Height = 30;
        }

        private void HideCheckList()
        {
            ModList.Height = 345;
            CheckListBorder.Height = 0;
        }

        private void ProcessCheckList()
        {
            if (DataProvider.SelectedServer == null || DataProvider.IsGameInProgress())
                return;
            try
            {
                var runs = new List<object[]>
                {
                    GenerateRuns("Оперативная память - всего", Client.CheckList.CheckMaxRam(DataProvider.SelectedServer)),
                    GenerateRuns("Оперативная память - свободная", Client.CheckList.CheckCurrentRam(DataProvider.SelectedServer)),
                    GenerateRuns("Файл подкачки", Client.CheckList.CheckPageFile(DataProvider.SelectedServer)),
                    GenerateRuns("64-битная система", Client.CheckList.CheckBits(DataProvider.SelectedServer)),
                    //GenerateRuns("Процессор", Client.CheckList.CheckCPU(DataProvider.SelectedServer)),
                    GenerateRuns("Место на диске ("+Config.WorkingDir.Root.Name+")", Client.CheckList.CheckFreeSpace(DataProvider.SelectedServer))
                };

                var warnLevel = runs.ConvertAll(it => (int) it[2]).Max();

                CheckList.Document.Blocks.Clear();
                var paragraph = new Paragraph();
                for (var i = 0; i < runs.Count; i++)
                {
                    paragraph.Inlines.Add((Run) runs[i][0]);
                    paragraph.Inlines.Add((Run) runs[i][1]);
                    if (i < runs.Count - 1)
                        paragraph.Inlines.Add("\n");
                }

                CheckList.Document.Blocks.Add(paragraph);

                if (warnLevel == 0)
                {
                    CheckListTitle.Text = "Проблем не обнаружено";
                    CheckListBorder.BorderBrush = (SolidColorBrush) new BrushConverter().ConvertFrom("#770F930F");
                    CheckListGrid.Background = (SolidColorBrush) new BrushConverter().ConvertFrom("#7737A337");
                    CheckListButton.Background = (SolidColorBrush) new BrushConverter().ConvertFrom("#CC0F930F");
                    CheckListButtonText.Text = "X";
                }
                else if (warnLevel == 1)
                {
                    CheckListTitle.Text = "Найдены потенциальные проблемы!";
                    CheckListBorder.BorderBrush = (SolidColorBrush) new BrushConverter().ConvertFrom("#77C8BC13");
                    CheckListGrid.Background = (SolidColorBrush) new BrushConverter().ConvertFrom("#778FA337");
                    CheckListButton.Background = (SolidColorBrush) new BrushConverter().ConvertFrom("#CCC8BC13");
                    CheckListButtonText.Text = "-";
                    if (CheckListBorder.Height < 5)
                        ShowCheckList();
                }
                else if (warnLevel == 2)
                {
                    CheckListTitle.Text = "Найдены вероятные проблемы!";
                    CheckListBorder.BorderBrush = (SolidColorBrush) new BrushConverter().ConvertFrom("#77C84113");
                    CheckListGrid.Background = (SolidColorBrush) new BrushConverter().ConvertFrom("#77A37537");
                    CheckListButton.Background = (SolidColorBrush) new BrushConverter().ConvertFrom("#FFC84113");
                    CheckListButtonText.Text = "-";
                    if (CheckListBorder.Height < 5)
                        ShowCheckList();
                }
            }
            catch
            {
            }
        }

        private void CheckListButtonClick(object sender, RoutedEventArgs routedEventArgs)
        {
            if (CheckListButtonText.Text == "-")
            {
                CheckListButtonText.Text = "+";
                MininizeCheckList();
            }
            else if (CheckListButtonText.Text == "+")
            {
                CheckListButtonText.Text = "-";
                ShowCheckList();
            }
            else
                HideCheckList();
        }

        private object[] GenerateRuns(string titie, CheckList.CheckResult result)
        {
            var run1 = GetSign(result.WarnLevel);
            run1.FontSize = 11;
            var run2 = new Run(" " + titie);
            run2.FontSize = 11;
            if (result.Message != "")
            {
                run1.ToolTip = new ToolTip {Content = result.Message};
                run2.ToolTip = new ToolTip {Content = result.Message};
                run1.MouseEnter += (s, e) => { ((ToolTip) run1.ToolTip).IsOpen = true; };
                run1.MouseLeave += (s, e) => { ((ToolTip) run1.ToolTip).IsOpen = false; };
                run2.MouseEnter += (s, e) => { ((ToolTip) run2.ToolTip).IsOpen = true; };
                run2.MouseLeave += (s, e) => { ((ToolTip) run2.ToolTip).IsOpen = false; };
            }

            return new object[] {run1, run2, result.WarnLevel};
        }

        public void ProcessServers(IEnumerable<ServerInfo> servers, NDictionary<string, string> links)
        {
            ProcessCheckList();
            var i = 0;
            foreach (var server in servers)
            {
                var textBlock = new TextBlock
                {
                    FontSize = 18,
                    Foreground = new SolidColorBrush(Color.FromArgb(181, 159, 140, 123)),
                    Text = server.ClientProfile.GetTitle(),
                    FontWeight = FontWeights.Bold,
                    VerticalAlignment = VerticalAlignment.Center,
                    Padding = new Thickness(15,9,0,0),
                    Width = 175,
                    Height = 45,
                };

                var button = new Button
                {
                    Margin = new Thickness(22, 102 + 60 * i, 0, 0),
                    VerticalAlignment = VerticalAlignment.Top,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    Width = 175,
                    Height = 45,
                    Style = (Style) Application.Current.Resources["StartMenuButtons"],
                    Content = textBlock,
                    Tag = server.ClientProfile.GetTitle(),
                    Background = new ImageBrush
                    {
                        ImageSource = ImageCache.ServerButtonInactive
                    }
                };
                button.Click += ServerButtonClick;
                button.MouseEnter += MouseEnterServerButton;
                button.MouseLeave += MouseLeaveServerButton;

                MainGrid.Children.Add(button);
                i++;
            }

            i = 0;
            foreach (var link in links)
            {
                var textBlock = new TextBlock
                {
                    FontSize = 18,
                    Foreground = new SolidColorBrush(Color.FromArgb(181, 159, 140, 123)),
                    Text = link.Key,
                    FontWeight = FontWeights.Bold,
                    VerticalAlignment = VerticalAlignment.Center,
                    Padding = new Thickness(15,9,0,0),
                    Width = 175,
                    Height = 45,
                };

                var button = new Button
                {
                    Content = textBlock,
                    Margin = new Thickness(22, 402 + 60 * i, 0, 0),
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Width = 175,
                    Tag = link.Value,
                    Height = 45,
                    Style = (Style) Application.Current.Resources["StartMenuButtons"],
                    Background = new ImageBrush
                    {
                        ImageSource = ImageCache.ServerButtonInactive
                    }
                };
                button.Click += LinkButtonClick;
                button.MouseEnter += MouseEnterServerButton;
                button.MouseLeave += MouseLeaveServerButton;

                MainGrid.Children.Add(button);
                i++;
            }
        }

        public void UpdateDisplay()
        {
            SetCurrentNews(0);
            SetCurrentChangeLog(0);

            var tags = DataProvider.SelectedServer.Tags;
            var title = DataProvider.SelectedServer.ClientProfile.GetTitle();
            ServerName.Content = title;

            if (DataProvider.SelectedServer.Slots=="?")
                PlayerCount.Content = "СЕРВЕР ВЫКЛЮЧЕН";
            else if (int.Parse(DataProvider.SelectedServer.Online)<3)
                PlayerCount.Content = "";
            else
                PlayerCount.Content = "Игроков: " + DataProvider.SelectedServer.Online + "/" + DataProvider.SelectedServer.Slots;

            var i = 0;
            foreach (var entry in tags)
            {
                try
                {
                    var border = (Border) FindName("TagBorder" + i);
                    border.Visibility = Visibility.Visible;
                    SolidColorBrush brush;
                    try
                    {
                        brush = new SolidColorBrush((Color) ColorConverter.ConvertFromString((string) entry.Value["color"]));
                    }
                    catch (Exception e)
                    {
                        App.WriteException(e, "entry=" + entry);
                        GoogleAnalytics.Exception("server_info:color", null, entry.ToString(), false);
                        brush = new SolidColorBrush(Color.FromRgb(200, 200, 200));
                    }

                    brush.Opacity = 0.5;
                    border.Background = brush;
                    ((Label) FindName("Tag" + i)).Content = entry.Key.Trim();
                    border.ToolTip = entry.Value["desc"];
                    i++;
                }
                catch (Exception e)
                {
                    App.WriteException( e, "entry="+entry);
                    GoogleAnalytics.Exception("server_info", null, entry.ToString(), false);
                }
            }

            for (; i < 7; i++)
            {
                try
                {
                    ((Border) FindName("TagBorder" + i)).Visibility = Visibility.Hidden;
                }
                catch (Exception e)
                {
                    App.WriteException(e, i.ToString());
                    GoogleAnalytics.Exception("server_info:tag", null, i.ToString(), false);
                }
            }

            var mods = new FlowDocument();

            foreach (var categoryEntry in DataProvider.SelectedServer.Mods)
            {
                try
                {
                    var paragraph = new Paragraph();
                    paragraph.Inlines.Add(new Run(categoryEntry.Key));
                    paragraph.LineHeight = 5;
                    mods.Blocks.Add(paragraph);

                    var modObject = new NDictionary<string, JToken>();
                    foreach (var modEntry in (JObject) categoryEntry.Value)
                        modObject.Add(modEntry.Key, modEntry.Value);

                    foreach (var modEntry in modObject.OrderBy(it => it.Key))
                    {
                        paragraph = new Paragraph();
                        paragraph.Inlines.Add(new Run("  " + modEntry.Key));
                        paragraph.LineHeight = 1;
                        mods.Blocks.Add(paragraph);
                        var inline = new Bold(new Run(" [?]"));
                        inline.MouseEnter += ModHelpMouseEnter;
                        inline.MouseLeave += ModHelpMouseLeave;
                        inline.MouseDown += ModHelpButtonClick;
                        paragraph.Inlines.Add(inline);
                        paragraph.LineHeight = 1;
                        paragraph.Tag = modEntry.Value;
                        mods.Blocks.Add(paragraph);
                    }

                    paragraph.Inlines.Add(new Run("\n"));
                }
                catch (Exception e)
                {
                    App.WriteException(e, categoryEntry.ToString());
                    GoogleAnalytics.Exception("server_info:category", null, categoryEntry.ToString(), true);
                }
            }
            ModList.Document = mods;
        }

        private void SetCurrentNews(int value)
        {
            currentNew = value;
            var news = DataProvider.SelectedServer.News[currentNew];
            DisplayMyNew();
            news.ImageUpdated += DisplayMyNew;
        }

        private void DisplayMyNew()
        {
            var news = DataProvider.SelectedServer.News[currentNew];
            ((ImageBrush) MainSlider.Background).ImageSource = news.Image ?? ImageCache.Nope;

            MainSliderText.Document.Blocks.Clear();
            var paragraph = new Paragraph();

            var niu = news.DateTime.CompareTo(Settings.GetLastVisit()) > 0;
            if (niu)
            {
                var effect = new DropShadowEffect
                {
                    BlurRadius = 22,
                    Color = Color.FromArgb(255, 229, 179, 79),
                    ShadowDepth = 0,
                    Opacity = 0.75
                };
                MainSliderBorder.Effect = effect;
            }
            else
            {
                MainSliderBorder.Effect = null;
            }

            Color.FromArgb(255, 229, 179, 79);
            
            paragraph.Inlines.Add(new Bold(new Run(news.Title)));
            if (niu)
            {
                var textBlock = new Bold(new Run("  NEW!"));
                textBlock.Foreground = new SolidColorBrush(Color.FromArgb(255, 229, 179, 79));
                paragraph.Inlines.Add(textBlock);
            }
            paragraph.Inlines.Add(new Run("\n " + news.Text));
            paragraph.Background = Brushes.Transparent;
            MainSliderText.Document.Blocks.Add(paragraph);
        }

        private void SetCurrentChangeLog(int value)
        {
            currentChangeLog = value;
            for (var i = 0; i < 3; i++)
            {
                try
                {
                    var fuckSharp = i;
                    var change = DataProvider.SelectedServer.ChangeLogs[currentChangeLog + fuckSharp];
                    DrawMyChangelog(fuckSharp);
                    change.ImageUpdated += () => DrawMyChangelog(fuckSharp);
                }
                catch
                {
                }
            }
        }

        private void DrawMyChangelog(int index)
        {
            var change = DataProvider.SelectedServer.ChangeLogs[currentChangeLog + index];
            var imageBox = (Grid) FindName("BottomSlider" + index);
            var textBox = (RichTextBox)FindName("BottomSliderText" + index);
            var newSplash = (TextBlock)FindName("BottomSliderNew" + index);
            ((ImageBrush) imageBox.Background).ImageSource = change.Image ?? ImageCache.Nope;

            textBox.Document.Blocks.Clear();

            if (change.DateTime.CompareTo(Settings.GetLastVisit()) > 0)
            {
                var effect = new DropShadowEffect
                {
                    BlurRadius = 12,
                    Color = Color.FromArgb(255, 229, 179, 79),
                    ShadowDepth = 0,
                    Opacity = 0.75
                };
                imageBox.Effect = effect;
                newSplash.Visibility = Visibility.Visible;
            }
            else
            {
                imageBox.Effect = null;
                newSplash.Visibility = Visibility.Hidden;
            }

            var paragraph = new Paragraph();
            paragraph.Inlines.Add(new Bold(new Run(change.Title + "\n")));
            if (change.Text.Trim().Length>0)
                paragraph.Inlines.Add(new Run(" " + change.Text));
            paragraph.FontSize = 10;
            textBox.Document.Blocks.Add(paragraph);
        }

        private void GotFocusHandler(object sender, RoutedEventArgs e)
        {
            foreach (Window window in Application.Current.Windows)
            {
                window.WindowState = WindowState.Normal;
                window.Focus();
            }
        }
    }
}
