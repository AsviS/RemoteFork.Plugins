﻿using System.Collections.Generic;
using System.Reflection;
using RemoteFork.Plugins.Settings;

namespace RemoteFork.Plugins {
    public class GetRootListCommand : ICommand {
        public List<Item> GetItems(IPluginContext context = null, params string[] data) {
            var items = new List<Item>();

            if (context != null) {
                string latestVersion =
                    context.GetLatestVersionNumber(typeof(Kinosha).GetCustomAttribute<PluginAttribute>().Id);
                if (!string.IsNullOrEmpty(latestVersion)) {
                    if (latestVersion != typeof(Kinosha).GetCustomAttribute<PluginAttribute>().Version) {
                        var updateItem = new Item() {
                            Name = $"Доступна новая версия: {latestVersion}",
                            Link = "http://newversion.m3u",
                            ImageLink = PluginSettings.Settings.Icons.NewVersion
                        };
                        items.Add(updateItem);
                    }
                }
            }

            var item = new Item() {
                Name = "Поиск",
                Type = ItemType.DIRECTORY,
                Link = $"{SearchCommand.KEY}",
                SearchOn = "Поиск",
                ImageLink = PluginSettings.Settings.Icons.IcoSearch
            };
            items.Add(item);

            item = new Item() {
                Name = "Новинки",
                Type = ItemType.DIRECTORY,
                Link = $"{GetCategoryCommand.KEY}{PluginSettings.Settings.Separator}{PluginSettings.Settings.Links.Site}/filmi-2018/",
                ImageLink = PluginSettings.Settings.Icons.IcoFolder
            };
            items.Add(item);

            item = new Item() {
                Name = "Фильмы",
                Type = ItemType.DIRECTORY,
                Link = $"{GetCategoryCommand.KEY}{PluginSettings.Settings.Separator}{PluginSettings.Settings.Links.Site}/films/",
                ImageLink = PluginSettings.Settings.Icons.IcoFolder
            };
            items.Add(item);

            item = new Item() {
                Name = "Сериалы",
                Type = ItemType.DIRECTORY,
                Link = $"{GetCategoryCommand.KEY}{PluginSettings.Settings.Separator}{PluginSettings.Settings.Links.Site}/serial/",
                ImageLink = PluginSettings.Settings.Icons.IcoFolder
            };
            items.Add(item);

            item = new Item() {
                Name = "Мультфильмы",
                Type = ItemType.DIRECTORY,
                Link = $"{GetCategoryCommand.KEY}{PluginSettings.Settings.Separator}{PluginSettings.Settings.Links.Site}/cartoons/",
                ImageLink = PluginSettings.Settings.Icons.IcoFolder
            };
            items.Add(item);

            item = new Item() {
                Name = "ТВ",
                Type = ItemType.DIRECTORY,
                Link = $"{GetCategoryCommand.KEY}{PluginSettings.Settings.Separator}{PluginSettings.Settings.Links.Site}/tv/",
                ImageLink = PluginSettings.Settings.Icons.IcoFolder
            };
            items.Add(item);

            return items;
        }
    }
}