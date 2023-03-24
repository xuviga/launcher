using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using iMine.Launcher.Request.Auth;
using iMine.Launcher.Utils;
using System.Windows.Controls;

namespace iMine.Launcher.Windows
{
    public partial class RegisterWindow
    {
        public RegisterWindow()
        {
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

            RegisterButton.Background = new ImageBrush
            {
                ImageSource = ImageCache.FancyButton
            };

            LoginButton.Background = new ImageBrush
            {
                ImageSource = ImageCache.GoldButton
            };

            CloseButton.Background = new ImageBrush
            {
                ImageSource = ImageCache.CloseButtonOnly
            };
            CloseButton.Click += (s, e) => { Close(); };

            MainWindow.Instance.BlackTint.Visibility = Visibility.Visible;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            MainWindow.Instance.Focus();
            GoogleAnalytics.ScreenView("main");
        }

        private void RegisterButtonEnter(object sender, MouseEventArgs e)
        {
            ((ImageBrush)RegisterButton.Background).ImageSource = ImageCache.FancyButtonHover;
        }

        private void RegisterButtonLeave(object sender, MouseEventArgs e)
        {
            ((ImageBrush)RegisterButton.Background).ImageSource = ImageCache.FancyButton;
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

        private void RegisterButtonClick(object sender, RoutedEventArgs ev)
        {
            ErrorBox.Visibility = Visibility.Hidden;
            try
            {
                if (PasswordBox.Password != PasswordAgainBox.Password)
                {
                    ErrorBox.Text = "Пароли не совпадают";
                    ErrorBox.Visibility = Visibility.Visible;
                    return;
                }

                DataProvider.AuthResult = new RegisterRequest(LoginBox.Text, PasswordBox.Password, EmailBox.Text).SendRequest();
                Settings.Username = LoginBox.Text;
                Settings.Token = DataProvider.AuthResult.AccessToken;
                MainWindow.Instance.ToggleLogin(false);
                Close();
            }
            catch (Exception ex)
            {
                switch (ex.Message)
                {
                    case "used_username":
                        ErrorBox.Text = "Такой логин занят";
                        break;
                    case "wrong_username":
                        ErrorBox.Text = "Логин может включать только латиницу, цифры и подчерк";
                        break;
                    default:
                        ErrorBox.Text = "Что-то пошло не так. Подробности в консоли.";
                        App.WriteException(ex);
                        GoogleAnalytics.Exception("register", null, "", true);
                        break;
                }
                new Thread(() => { App.WriteLog(ErrorBox.Text); }).Start();
                ErrorBox.Visibility = Visibility.Visible;
            }
        }
    }
}
