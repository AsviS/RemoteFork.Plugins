using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace RemoteFork.Plugins {
    [PluginAttribute(Id = "godzfilm", Version = "0.0.1", Author = "fd_crash", Name = "GodZfilm",
        Description = "Смотреть онлайн бесплатно фильмы, сериалы, аниме, мультфильмы.",
        ImageLink = "http://s1.iconbird.com/ico/2013/6/353/w256h2561372333145videoicon.png")]
    public class GodZfilm : IPlugin {
        public static readonly Dictionary<string, List<Match>> SERIAL_MATCHES = new Dictionary<string, List<Match>>();
        public static readonly Dictionary<string, Item> SERIAL_ITEMS = new Dictionary<string, Item>();
        
        public const char SEPARATOR = ';';
        public const string PLUGIN_PATH = "pluginPath";
        public static string NextPageUrl = null;
        
        public Playlist GetList(IPluginContext context) {
            string path = context.GetRequestParams().Get(PLUGIN_PATH);

            path = path == null ? "plugin" : "plugin;" + path;

            var arg = path.Split(SEPARATOR);

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
                string[] data = new string[Math.Max(5, arg.Length)];
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
                var pluginParams = new NameValueCollection {[PLUGIN_PATH] = NextPageUrl};
                playlist.NextPageUrl = context.CreatePluginUrl(pluginParams);
            } else {
                playlist.NextPageUrl = null;
            }

            foreach (var item in items) {
                if (ItemType.DIRECTORY == item.Type) {
                    var pluginParams = new NameValueCollection {
                        [PLUGIN_PATH] = item.Link
                    };

                    item.Link = context.CreatePluginUrl(pluginParams);
                }
            }

            playlist.Items = items.ToArray();

            return playlist;
        }
    }
}