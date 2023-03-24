using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using iMine.Launcher.Utils;

namespace iMine.Launcher.Windows
{
    public partial class ConsoleWindow
    {
        public static ConsoleWindow Instance;

        public ConsoleWindow()
        {
            Instance = this;
            InitializeComponent();

            CloseButton.Background = new ImageBrush
            {
                ImageSource = ImageCache.CloseButtonOnly
            };
            CloseButton.Click += (s, e) =>
            {
                if (MainWindow.Instance == null || !MainWindow.Instance.IsVisible)
                {
                    GoogleAnalytics.EndSession("console");
                    App.IsShuttingDown = true;
                }
                else
                    Hide();
            };
            KillButton.Background = new ImageBrush
            {
                ImageSource = ImageCache.GoldButtonHover
            };
        }

        private void KillButtonClick(object sender, RoutedEventArgs e)
        {
            if (MainWindow.Instance == null || !MainWindow.Instance.IsVisible)
            {
                GoogleAnalytics.EndSession("console");
                App.IsShuttingDown = true;
            }

            if (DataProvider.GameProcess!=null && !DataProvider.GameProcess.HasExited)
                ClientLauncher.CloseGame();
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

        private void OnDrag(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void GoldButtonEnter(object sender, MouseEventArgs e)
        {
            if (DataProvider.GameProcess != null)
            {
                ((Button)sender).Background = new ImageBrush
                {
                    ImageSource = ImageCache.GoldButtonHover
                };
            }
        }

        private void GoldButtonLeave(object sender, MouseEventArgs e)
        {
            if (DataProvider.GameProcess != null)
            {
                ((Button)sender).Background = new ImageBrush
                {
                    ImageSource = ImageCache.GoldButton
                };
            }
        }
    }
}
