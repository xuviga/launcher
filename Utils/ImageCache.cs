using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using Newtonsoft.Json.Linq;

namespace iMine.Launcher.Utils
{
    public static class ImageCache
    {
        public static readonly BitmapImage Nope = Load(new Uri("assets/nope.png", UriKind.Relative));
        public static readonly BitmapImage MainBcg = Load(new Uri("assets/main_bcg.png", UriKind.Relative));
        public static readonly BitmapImage MainBcgTint = Load(new Uri("assets/main_bcg_tint.png", UriKind.Relative));
        public static readonly BitmapImage Console = Load(new Uri("assets/console.png", UriKind.Relative));
        public static readonly BitmapImage Bug = Load(new Uri("assets/bug.png", UriKind.Relative));
        public static readonly BitmapImage Cog = Load(new Uri("assets/cog.png", UriKind.Relative));
        public static readonly BitmapImage Logo = Load(new Uri("assets/imine_logo.png", UriKind.Relative));
        public static readonly BitmapImage ServerButtonInactive = Load(new Uri("assets/server_button_inactive.png", UriKind.Relative));
        public static readonly BitmapImage ServerButtonActive = Load(new Uri("assets/server_button_active.png", UriKind.Relative));
        public static readonly BitmapImage PlayButtonInactive = Load(new Uri("assets/play_button_inactive.png", UriKind.Relative));
        public static readonly BitmapImage PlayButtonActive = Load(new Uri("assets/play_button_active.png", UriKind.Relative));
        public static readonly BitmapImage GoldButton = Load(new Uri("assets/gold_button.png", UriKind.Relative));
        public static readonly BitmapImage GoldButtonHover = Load(new Uri("assets/gold_button_hover.png", UriKind.Relative));
        public static readonly BitmapImage DiamondButton = Load(new Uri("assets/diamond_button.png", UriKind.Relative));
        public static readonly BitmapImage DiamondButtonHover = Load(new Uri("assets/diamond_button_hover.png", UriKind.Relative));
        public static readonly BitmapImage CloseButton = Load(new Uri("assets/close_button.png", UriKind.Relative));
        public static readonly BitmapImage CloseButtonOnly = Load(new Uri("assets/close_button_only.png", UriKind.Relative));
        public static readonly BitmapImage CloseButtonGlowing = Load(new Uri("assets/close_button_glowing.png", UriKind.Relative));
        public static readonly BitmapImage MinimizeButtonGlowing = Load(new Uri("assets/min_button_glowing.png", UriKind.Relative));
        public static readonly BitmapImage CloseButtonOnlyGlowing = Load(new Uri("assets/close_button_only_glowing.png", UriKind.Relative));
        public static readonly BitmapImage ModListPanel = Load(new Uri("assets/modlist.png", UriKind.Relative));
        public static readonly BitmapImage FancyButton = Load(new Uri("assets/fancy_button.png", UriKind.Relative));
        public static readonly BitmapImage FancyButtonHover = Load(new Uri("assets/fancy_button_hover.png", UriKind.Relative));
        public static readonly BitmapImage TintMask = Load(new Uri("assets/tint_mask.png", UriKind.Relative));
        public static readonly BitmapImage BlankDialog = Load(new Uri("assets/blank_dialog.png", UriKind.Relative));
        public static readonly BitmapImage LoadingBcg = Load(new Uri("assets/loading_bcg.png", UriKind.Relative));
        public static readonly BitmapImage LoadingBar = Load(new Uri("assets/loading_bar.png", UriKind.Relative));

        private static readonly DirectoryInfo CacheDir = Config.WorkingDir.ResolveDirectory("Launcher/Cache");
        private static JObject _cacheDic;

        public static void Init()
        {
            try
            {
                CacheDir.Create();
                _cacheDic = JObject.Parse(File.ReadAllText(CacheDir.ResolveFile("dictionary.cfg").FullName));

                var clone = (JObject)_cacheDic.DeepClone();
                foreach (var entry in clone)
                {
                    var data = (JObject) entry.Value;
                    var datTime = DateTime.FromBinary(long.Parse(data["time"].ToString()));
                    if ((DateTime.Now - datTime).TotalDays > 7)
                    {
                        CacheDir.ResolveFile(data["file"].ToString()).Delete();
                        _cacheDic.Remove(entry.Key);
                    }
                }
            }
            catch
            {
                _cacheDic = new JObject();
                CacheDir.Delete(true);
                Thread.Sleep(3000);
                CacheDir.Create();
            }
        }

        private static BitmapImage Load(Uri path)
        {
            try
            {
                var imageData = File.ReadAllBytes(path.ToString());
                var stream = new MemoryStream(imageData);
                var image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = stream;
                image.EndInit();
                return image;
            }
            catch (Exception e)
            {
                App.WriteImportantException(e, "Не удалось загрузить изображение " + path);
                GoogleAnalytics.Exception("load_resource",null,path.ToString(),false);
                return null;
            }
        }

        public static byte[] GetImageFromCache(string imageUrl)
        {
            lock (Application.Current)
            {
                try
                {
                    if (_cacheDic.ContainsKey(imageUrl))
                    {
                        var data = (JObject) _cacheDic[imageUrl];
                        data["time"] = DateTime.Now.ToBinary();
                        File.WriteAllText(CacheDir.ResolveFile("dictionary.cfg").FullName, _cacheDic.ToString());
                        return File.ReadAllBytes(CacheDir.ResolveFile(data["file"].ToString()).FullName);
                    }
                }
                catch
                {
                }
                return null;
            }
        }

        public static void PutImageToCache(string imageUrl, byte[] imageData)
        {
            lock (Application.Current)
            {
                if (_cacheDic.ContainsKey(imageUrl))
                    return;
                try
                {
                    var newFileName = Guid.NewGuid() + ".png";
                    var data = new JObject
                    {
                        ["file"] = newFileName,
                        ["time"] = DateTime.Now.ToBinary()
                    };
                    _cacheDic[imageUrl] = data;
                    File.WriteAllBytes(CacheDir.ResolveFile(newFileName).FullName, imageData);
                    File.WriteAllText(CacheDir.ResolveFile("dictionary.cfg").FullName, _cacheDic.ToString());
                }
                catch
                {
                }
            }
        }
    }
}