﻿using System.Collections.Generic;
using System.Collections.Specialized;
using RemoteFork.Plugins.Commands;
using RemoteFork.Plugins.Settings;

namespace RemoteFork.Plugins {
    [Plugin(Id = "youtube", Version = "0.1.2", Author = "fd_crash", Name = "YouTube",
        Description = "Видеохостинговая компания, предоставляющая пользователям услуги хранения, доставки и показа видео. Пользователи могут загружать, просматривать, оценивать, комментировать, добавлять в избранное и делиться теми или иными видеозаписями",
        ImageLink = "https://img.icons8.com/flat_round/384/youtube-play.png",
        Github = "ShutovPS/RemoteFork.Plugins/YouTube")]

    public class YouTube : IPlugin {
        public static bool IsIptv = false;
        public static string NextPageUrl = null;
        public static string Source = null;

        public Playlist GetList(IPluginContext context) {
            string path = context.GetRequestParams().Get(PluginSettings.Settings.PluginPath);

            path = path == null ? "plugin" : "plugin;" + path;

            var arg = path.Split(PluginSettings.Settings.Separator);

            var items = new List<Item>();
            ICommand command = null;
            var data = new string[4];
            switch (arg.Length) {
                case 0:
                    break;
                case 1:
                    command = new GetRootListCommand();
                    break;
                default:
                    switch (arg[1]) {
                        case "list":
                            command = new GetListCommand();
                            break;
                        case "search":
                            command = new GetSearchCommand();
                            break;
                        case "video":
                            command = new GetVideoCommand();
                            break;
                        case "channel":
                            command = new GetChannelCommand();
                            break;
                        case "playlist":
                            command = new GetPlaylistCommand();
                            break;
                    }
                    break;
            }

            NextPageUrl = Source = null;
            IsIptv = false;

            if (command != null) {
                for (int i = 0; i < arg.Length; i++) {
                    data[i] = arg[i];
                }
                items.AddRange(command.GetItems(context, data));
            }

            return CreatePlaylist(items, context);
        }

        public static Playlist CreatePlaylist(List<Item> items, IPluginContext context) {
            var playlist = new Playlist();

            if (!string.IsNullOrEmpty(NextPageUrl)) {
                var pluginParams = new NameValueCollection {[PluginSettings.Settings.PluginPath] = NextPageUrl };
                playlist.NextPageUrl = context.CreatePluginUrl(pluginParams);
            }
            playlist.Timeout = "60";

            playlist.Items = items.ToArray();

            foreach (var item in playlist.Items) {
                if (ItemType.DIRECTORY == item.Type) {
                    var pluginParams = new NameValueCollection {
                        [PluginSettings.Settings.PluginPath] = item.Link
                    };
                    item.Link = context.CreatePluginUrl(pluginParams);
                }
            }

            playlist.source = Source;
            playlist.IptvPlaylist = IsIptv;

            return playlist;
        }
    }
}
