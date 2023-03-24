using System;
using System.Windows;
using System.Windows.Media;
using iMine.Launcher.Utils;

namespace iMine.Launcher.Windows
{
    public partial class LoadingWindow
    {
        public static LoadingWindow Instance { get; private set; }

        public LoadingWindow()
        {
            Instance = this;
            GoogleAnalytics.StartSession();
            Focus();
            InitializeComponent();
            Background = new ImageBrush
            {
                ImageSource = ImageCache.Logo
            };
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (MainWindow.Instance == null || MainWindow.Instance.Visibility != Visibility.Visible)
            {
                GoogleAnalytics.EndSession("loading");
                App.IsShuttingDown = true;
            }
        }
    }
}
