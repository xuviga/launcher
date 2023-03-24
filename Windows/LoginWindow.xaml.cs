using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using iMine.Launcher.Client;
using iMine.Launcher.Request.Auth;
using iMine.Launcher.Utils;

namespace iMine.Launcher.Windows
{
    public partial class LoginWindow
    {
        public static LoginWindow Instance { get; private set; }

        public ServerInfo WannaStartGameAfter;

        public LoginWindow()
        {
            Instance = this;
            InitializeComponent();
            PostInitializeComponent();
            SecondaryWindowController.HookWindow(this);

            Left = MainWindow.Instance.Left + (MainWindow.Instance.Width - Width) / 2;
            Top = MainWindow.Instance.Top + (MainWindow.Instance.Height - Height) / 2;
            Owner = MainWindow.Instance;
        }

        private void PostInitializeComponent()
        {
            var ratio = SystemParameters.PrimaryScreenWidth / 1920 * 1.3;
            Width *= ratio;
            Height *= ratio;

            Background = new ImageBrush
            {
                ImageSource = ImageCache.BlankDialog
            };

            ResetPassButton.Background = NoAccountButton.Background = new ImageBrush
            {
                ImageSource = ImageCache.GoldButton
            };

            CloseButton.Background = new ImageBrush
            {
                ImageSource = ImageCache.CloseButtonOnly
            };
            CloseButton.Click += (s, e) => { Close(); };

            LoginButton.Background = new ImageBrush
            {
                ImageSource = ImageCache.FancyButton
            };
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            MainWindow.Instance.Focus();
            GoogleAnalytics.ScreenView("main");
        }

        private void RegisterButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
            GoogleAnalytics.ScreenView("register");
            new RegisterWindow().Show();
        }

        private void ForgotButtonClick(object sender, RoutedEventArgs e)
        {
            ClientLauncher.OpenWebPage(Config.OurWebsite.ToString());
        }

        private void GoldButtonEnter(object sender, MouseEventArgs e)
        {
            ((Button)sender).Background = new ImageBrush
            {
                ImageSource = ImageCache.GoldButtonHover
            };
        }

        private void GoldButtonLeave(object sender, MouseEventArgs e)
        {
            ((Button)sender).Background = new ImageBrush
            {
                ImageSource = ImageCache.GoldButton
            };
        }

        private void CloseButtonEnter(object sender, MouseEventArgs e)
        {
            ((Button)sender).Background = new ImageBrush
            {
                ImageSource = ImageCache.CloseButtonOnlyGlowing
            };
        }

        private void CloseButtonLeave(object sender, MouseEventArgs e)
        {
            ((Button)sender).Background = new ImageBrush
            {
                ImageSource = ImageCache.CloseButtonOnly
            };
        }

        private void LoginButtonClick(object sender, RoutedEventArgs ev)
        {
            ErrorBox.Text = "";
            ErrorBox.Visibility = Visibility.Hidden;
            try
            {
                DataProvider.AuthResult = new AuthRequest(LoginBox.Text, PasswordBox.Password).SendRequest();
                Settings.Username = LoginBox.Text;
                Settings.Token = DataProvider.AuthResult.AccessToken;
                MainWindow.Instance.ToggleLogin(false);
                if (WannaStartGameAfter != null && !SecondaryWindowController.Windows.Any(it=>it is DownloadingWindow))
                {
                    new Thread(() => { Config.RunClient(WannaStartGameAfter); }).Start();
                }

                Close();
            }
            catch(Exception ex)
            {
                DataProvider.AuthResult = null;
                Settings.Token = null;
                switch (ex.Message)
                {
                    case "wrong_username":
                        ErrorBox.Text = "Некорректное имя пользователя";
                        break;
                    case "wrong_password":
                        ErrorBox.Text = "Неправильная пара логин-пароль";
                        break;
                    default:
                        ErrorBox.Text = "Что-то пошло не так. Подробности в консоли.";
                        App.WriteException(ex,"Что-то пошло не так. Подробности в консоли.");
                        GoogleAnalytics.Exception("login",null,"",true);
                        break;
                }
                App.WriteLog(ErrorBox.Text);
                ErrorBox.Visibility = Visibility.Visible;
            }
        }

        private void LoginButtonEnter(object sender, MouseEventArgs e)
        {
            ((ImageBrush)LoginButton.Background).ImageSource = ImageCache.FancyButtonHover;
        }

        private void LoginButtonLeave(object sender, MouseEventArgs e)
        {
            ((ImageBrush)LoginButton.Background).ImageSource = ImageCache.FancyButton;
        }
    }
}
