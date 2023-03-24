using System;
using System.Windows;
using iMine.Launcher.Utils;
using System.Windows.Media;
using System.Windows.Threading;
using iMine.Launcher.Request.Update;
using System.Windows.Controls;

namespace iMine.Launcher.Windows
{
    public partial class DownloadingWindow
    {
        public UpdateRequest Request;
        private int busyBarTick;
        public bool ImOk;

        public DownloadingWindow()
        {
            InitializeComponent();

            var ratio = SystemParameters.PrimaryScreenWidth / 1920 * 1.3;
            Width *= ratio;
            Height *= ratio;

            SecondaryWindowController.HookWindow(this);

            Left = MainWindow.Instance.Left + (MainWindow.Instance.Width - Width) / 2;
            Top = MainWindow.Instance.Top + (MainWindow.Instance.Height - Height) / 3;
            Owner = MainWindow.Instance;

            Background = new ImageBrush
            {
                ImageSource = ImageCache.LoadingBcg
            };

            Slider.Fill = new ImageBrush
            {
                ImageSource = ImageCache.LoadingBar
            };

            CloseButton.Background = new ImageBrush
            {
                ImageSource = ImageCache.GoldButton
            };

            var timer = new DispatcherTimer();
            timer.Tick += OnTimer;
            timer.Interval = new TimeSpan(0,0,0,0,50);
            timer.Start();
        }

        private void CloseButtonClick(object sender, EventArgs e)
        {
            Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            MainWindow.Instance.Focus();
            LoginWindow.Instance.WannaStartGameAfter = null;
            if (!ImOk)
                Config.StopDownloadingClient();
        }

        private void OnTimer(object sender, EventArgs e)
        {
            if (Request != null && Request.DownloadedSize > 0)
            {
                var ratio = Request.DownloadedSize / (double)Request.TotalSize;
                SliderBase.Width = Slider.Width * ratio;
                SliderBase.Margin=new Thickness(27,160,0,0);
                DownloadDesc.Text=$"Качаем файл: {Request.CurrentFile} ({Request.CurrentDownloadedSize / 1048576.0:0.00}/{Request.CurrentTotalSize / 1048576.0:0.00}MiB)\n" +
                                       $"{Request.DownloadedSize / 1048576.0:0.00}/{Request.TotalSize / 1048576.0:0.00}MiB";
            }
            else
            {
                if (busyBarTick>=Slider.Width+65)
                {
                    busyBarTick = 0;
                }
                var posX = Math.Max(busyBarTick - 43, 27);
                SliderBase.Margin=new Thickness(posX,160,0,0);
                SliderBase.Width = Math.Min(busyBarTick,70);
                busyBarTick += 5;
                if (busyBarTick>=Slider.Width-5)
                {
                    SliderBase.Width = 65 - (busyBarTick - Slider.Width);
                }
            }
        }

        private void CloseButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var button = (Button)sender;
            ((ImageBrush)button.Background).ImageSource = ImageCache.GoldButtonHover;
        }

        private void CloseButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var button = (Button)sender;
            ((ImageBrush)button.Background).ImageSource = ImageCache.GoldButton;
        }
    }
}
