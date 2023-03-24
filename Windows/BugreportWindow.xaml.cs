using System;
using iMine.Launcher.Utils;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace iMine.Launcher.Windows
{
    public partial class BugreportWindow : Window
    {
        private static BugreportWindow instance;
        private bool sending;

        public static void Show(bool bugButton)
        {
            GoogleAnalytics.ScreenView(bugButton ? "debug" : "debug_exception");
            if (instance == null || !instance.IsVisible)
                new BugreportWindow(bugButton).Show();
        }

        private BugreportWindow(bool bugButton)
        {
            instance = this;
            InitializeComponent();

            PostInitializeComponent();

            if (bugButton)
                ErrorDesc.Text = "Нашел баг? Расскажи о нем нам. Если нужна обратная связь, обращайся в наш Discord.";

            if (MainWindow.Instance != null && MainWindow.Instance.IsVisible)
            {
                Left = MainWindow.Instance.Left + (MainWindow.Instance.Width - Width) / 2;
                Top = MainWindow.Instance.Top + (MainWindow.Instance.Height - Height) / 2;
                Owner = MainWindow.Instance;
            }
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
            
            CloseButton.Background = new ImageBrush
            {
                ImageSource = ImageCache.CloseButtonOnly
            };
            CloseButton.Click += (s, e) => { Close(); };

            LoginButton.Background = new ImageBrush
            {
                ImageSource = ImageCache.FancyButton
            };

            try
            {
                LoginBox.Text = DataProvider.AuthResult.PlayerProfile.username;
                LoginBox.IsReadOnly = true;
            }
            catch
            {
            }
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
            if (SendButton.Content=="Успешно!")
                Close();
            else
                SendBugreport();
        }

        private void LoginButtonEnter(object sender, MouseEventArgs e)
        {
            ((ImageBrush)LoginButton.Background).ImageSource = ImageCache.FancyButtonHover;
        }

        private void LoginButtonLeave(object sender, MouseEventArgs e)
        {
            ((ImageBrush)LoginButton.Background).ImageSource = ImageCache.FancyButton;
        }

        private void SendBugreport()
        {
            if (sending)
                return;
            sending = true;
            try
            {
                if (GoogleAnalytics.SendBugreport(LoginBox.Text, DescBox.Text))
                    SendButton.Content = "Успешно!";
                else
                    SendButton.Content = "Неудачно!";
            }
            catch (Exception e)
            {
                App.WriteException(e,"У нас не получилось отправить отчет об ошибке!\nСвяжись с нами через соцсети! Ссылки на главном окне");
                GoogleAnalytics.Exception("bugreport",e,"",true);
                ErrorDesc.Text = "У нас не получилось отправить отчет об ошибке!\nСвяжись с нами через соцсети! Ссылки на главном окне";
                SendButton.Content = "Неудачно!";
            }
        }
    }
}
