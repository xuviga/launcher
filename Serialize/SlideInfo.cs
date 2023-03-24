using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using iMine.Launcher.Utils;

namespace iMine.Launcher.Serialize
{
    public class SlideInfo
    {
        public delegate void ImageUpdatedHandler();
        public event ImageUpdatedHandler ImageUpdated;

        public BitmapImage Image { get; private set; }
        public readonly string Title;
        public readonly string Text;
        public readonly string ClickUrl;
        public readonly DateTime DateTime;

        public SlideInfo(string title, string imageUrl, string text, string clickUrl, DateTime dateTime)
        {
            new Thread(()=>LoadImage(imageUrl))
            {
                IsBackground = true
            }.Start();
            Title = title;
            Text = text;
            ClickUrl = clickUrl;
            DateTime = dateTime;
        }

        private void LoadImage(string imageUrl)
        {
            lock (imageUrl)
            {
                try
                {
                    var imageData = ImageCache.GetImageFromCache(imageUrl);

                    if (imageData == null)
                    {
                        using (var client = new WebClient())
                        {
                            client.Proxy = GlobalProxySelection.GetEmptyWebProxy();
                            imageData = client.DownloadData(imageUrl);
                            ImageCache.PutImageToCache(imageUrl, imageData);
                        }
                    }

                    var stream = new MemoryStream(imageData);
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        var image = new BitmapImage();
                        image.BeginInit();
                        image.StreamSource = stream;
                        image.EndInit();
                        Image = image;
                        ImageUpdated?.Invoke();
                    }));
                }
                catch
                {
                }
            }
        }
    }
}