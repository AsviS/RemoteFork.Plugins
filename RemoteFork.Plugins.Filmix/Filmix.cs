using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using RemoteFork.Log;
using RemoteFork.Plugins.Settings;

namespace RemoteFork.Plugins {
    [PluginAttribute(Id = "filmix", Version = "0.0.3", Author = "fd_crash", Name = "Filmix",
        Description = "Смотреть онлайн фильмы и сериалы бесплатно.",
        ImageLink = "http://filmix.cc/templates/Filmix/media/img/filmix.png",
        Github = "ShutovPS/RemoteFork.Plugins/Filmix")]
    public class Filmix : IPlugin {
        public static readonly Logger Logger = new Logger(typeof(Filmix));

        public static string NextPageUrl = null;
        
        public Playlist GetList(IPluginContext context) {
            string path = context.GetRequestParams().Get(PluginSettings.Settings.PluginPath);

            path = path == null ? "plugin" : "plugin;" + path;

            var arg = path.Split(PluginSettings.Settings.Separator);

            var items = new List<Item>();
            ICommand command = null;
            switch (arg.Length) {
                case 0:
                    break;
                case 1:
                    command = new GetRootListCommand();
                    break;
                default:
                    switch (arg[1]) {
                        case SearchCommand.KEY:
                            command = new SearchCommand();
                            break;
                        case GetCategoryCommand.KEY:
                            command = new GetCategoryCommand();
                            break;
                        case GetFilmCommand.KEY:
                            command = new GetFilmCommand();
                            break;
                        case GetEpisodeCommand.KEY:
                            command = new GetEpisodeCommand();
                            break;
                    }

                    break;
            }

            NextPageUrl = null;

            if (command != null) {
                var data = new string[Math.Max(5, arg.Length)];
                for (int i = 0; i < arg.Length; i++) {
                    data[i] = arg[i];
                }

                items.AddRange(command.GetItems(context, data));
            }

            return CreatePlaylist(items, context);
        }

        private static Playlist CreatePlaylist(List<Item> items, IPluginContext context) {
            var playlist = new Playlist();

            if (!string.IsNullOrEmpty(NextPageUrl)) {
                var pluginParams = new NameValueCollection {[PluginSettings.Settings.PluginPath] = NextPageUrl};
                playlist.NextPageUrl = context.CreatePluginUrl(pluginParams);
            } else {
                playlist.NextPageUrl = null;
            }

            foreach (var item in items) {
                if (ItemType.DIRECTORY == item.Type) {
                    var pluginParams = new NameValueCollection {
                        [PluginSettings.Settings.PluginPath] = item.Link
                    };

                    item.Link = context.CreatePluginUrl(pluginParams);
                }
            }

            playlist.Items = items.ToArray();

            return playlist;
        }
    }
}
