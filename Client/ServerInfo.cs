using System;
using System.Collections.Generic;
using iMine.Launcher.Serialize;
using iMine.Launcher.Utils;
using Newtonsoft.Json.Linq;

namespace iMine.Launcher.Client
{
    public class ServerInfo
    {
        public readonly ClientProfile ClientProfile;
        public readonly List<SlideInfo> News;
        public readonly List<SlideInfo> ChangeLogs;
        public readonly JObject Mods;
        public readonly JObject Tags;
        public readonly string Online;
        public readonly string Slots;

        public ServerInfo(ClientProfile clientProfile)
        {
            ClientProfile = clientProfile;

            var news = new List<SlideInfo>();
            var changelogs = new List<SlideInfo>();

            var slide = new SlideInfo("Не удалось связаться с нашим сайтом", "assets/imine_logo.png", "Новости и список модов временно недоступны. Возможность играть сохранилась (если сервера не упали)", "", DateTime.MinValue);
            news.Add(slide);
            var slide2 = new SlideInfo("", "assets/imine_logo.png", "", "", DateTime.MinValue);
            changelogs.Add(slide2);
            changelogs.Add(slide2);
            changelogs.Add(slide2);

            News = news;
            ChangeLogs = changelogs;
            Mods = new JObject();
            Tags = new JObject();
            Slots = Online = "0";

        }

        public ServerInfo(ClientProfile clientProfile, JObject serverData, JArray newsData, JArray changelogsData)
        {
            ClientProfile = clientProfile;

            var news = new List<SlideInfo>();
            var changelogs = new List<SlideInfo>();

            var title = serverData.ContainsKey("title") ? serverData["title"].Value<string>() : "[Error]";
            var text = serverData.ContainsKey("text") ? serverData["text"].Value<string>() : "[Error]";
            var clickUrl = new Uri(Config.OurWebsite, "about/"+clientProfile.GetTitle().ToLower()).ToString();
            var imageUrl = serverData.ContainsKey("image") ? serverData["image"].Value<string>() : "";
            var tags = serverData.ContainsKey("tags") ? (JObject)serverData["tags"] : new JObject();
            Online = serverData.ContainsKey("online") ? serverData["online"].ToString() : "?";
            Slots = serverData.ContainsKey("slots") ? serverData["slots"].ToString() : "?";

            var timestamp = Settings.GetFirstLaunch(title);
            if (timestamp == DateTime.MinValue)
                timestamp = DateTime.Now;
            else
                timestamp = DateTime.MinValue;

            var slide = new SlideInfo(title, imageUrl, text, clickUrl, timestamp);
            news.Add(slide);

            var i = 0;
            foreach (var entry in newsData)
            {
                try
                {
                    title = entry["title"].Value<string>();
                    text = entry["text"].Value<string>();
                    clickUrl = new Uri(new Uri(Config.OurWebsite, "news/"), entry["click_url"].Value<string>()).ToString();
                    imageUrl = entry["image"].Value<string>();
                    timestamp = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                    timestamp = timestamp.AddMilliseconds(entry["timestamp"].Value<long>()).ToLocalTime();

                    slide = new SlideInfo(title, imageUrl, text, clickUrl, timestamp);
                    news.Add(slide);
                }
                catch (Exception e)
                {
                    App.WriteException(e, entry.ToString());
                    GoogleAnalytics.Exception("news", null, entry.ToString(), true);
                }

                if (i++ >= 5)
                    break;
            }

            foreach (var entry in changelogsData)
            {
                try
                {
                    title = entry["title"].Value<string>();
                    text = entry["text"].Value<string>();
                    clickUrl = new Uri(new Uri(Config.OurWebsite, "news/"), entry["click_url"].Value<string>()).ToString();
                    imageUrl = entry["image"].Value<string>();
                    timestamp = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                    timestamp = timestamp.AddMilliseconds(entry["timestamp"].Value<long>()).ToLocalTime();

                    slide = new SlideInfo(title, imageUrl, text, clickUrl, timestamp);
                    changelogs.Add(slide);
                }
                catch (Exception e)
                {
                    App.WriteException(e, entry.ToString());
                    GoogleAnalytics.Exception("changelogs", null, entry.ToString(), true);
                }
            }

            slide = new SlideInfo("Остальные новости смотри на нашем сайте imine.ru", "assets/imine_logo.png", "", "", DateTime.MinValue);
            changelogs.Add(slide);

            news.Sort((it,that)=>that.DateTime.CompareTo(it.DateTime));
            changelogs.Sort((it,that)=>that.DateTime.CompareTo(it.DateTime));
            News = news;
            ChangeLogs = changelogs;
            Mods = (JObject) serverData["mods"];
            Tags = tags;
        }

        public override string ToString()
        {
            return $"{ClientProfile} {News} {ChangeLogs} {Tags}";
        }
    }
}