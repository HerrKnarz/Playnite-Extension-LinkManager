﻿using KNARZhelper;
using Playnite.SDK;
using Playnite.SDK.Models;
using Playnite.SDK.Plugins;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WikipediaMetadata.Models;

namespace WikipediaMetadata
{
    public class WikipediaMetadataProvider : OnDemandMetadataProvider
    {
        private readonly MetadataRequestOptions options;
        private readonly WikipediaMetadata plugin;

        private WikipediaGameMetadata foundGame;

        public override List<MetadataField> AvailableFields => plugin.SupportedFields;

        public WikipediaMetadataProvider(MetadataRequestOptions options, WikipediaMetadata plugin)
        {
            this.options = options;
            this.plugin = plugin;
        }

        private WikipediaGameMetadata FindGame()
        {
            if (foundGame != null)
            {
                return foundGame;
            }

            WikipediaGameData page = new WikipediaGameData();
            string pageHtml = string.Empty;

            try
            {
                WikipediaSearchResult searchResult = WikipediaApiCaller.GetSearchResults(options.GameData.Name);

                string key = string.Empty;

                if (searchResult.Pages != null && searchResult.Pages.Count > 0)
                {
                    string wikiNameVideoGame = (options.GameData.Name + " (video game)").RemoveSpecialChars().ToLower().Replace(" ", "");
                    string wikiName = options.GameData.Name.RemoveSpecialChars().ToLower().Replace(" ", "");
                    string wikiStart = wikiName.Substring(0, (wikiName.Length > 5) ? 5 : wikiName.Length);


                    if (options.IsBackgroundDownload)
                    {
                        // TODO: Maybe look for existing wikipedia link and use that as the base!


                        Page foundPage = searchResult.Pages.Where(p => p.KeyMatch == wikiNameVideoGame).FirstOrDefault() ??
                            searchResult.Pages.Where(p => p.KeyMatch == options.GameData.Name.RemoveSpecialChars().ToLower().Replace(" ", "")).FirstOrDefault();

                        if (foundPage != null)
                        {
                            key = foundPage.Key;
                        }
                        else
                        {
                            key = string.Empty;
                        }
                    }
                    else
                    {

                        GenericItemOption chosen = plugin.PlayniteApi.Dialogs.ChooseItemWithSearch(null, s =>
                        {
                            return searchResult.Pages.Select(WikipediaItemOption.FromWikipediaSearchResult)
                                .OrderByDescending(o => o.Description != null && o.Description.Contains("video game"))
                                .ThenByDescending(o => o.Name.RemoveSpecialChars().ToLower().Replace(" ", "").StartsWith(wikiNameVideoGame))
                                .ThenByDescending(o => o.Name.RemoveSpecialChars().ToLower().Replace(" ", "").StartsWith(wikiStart))
                                .ThenByDescending(o => o.Name.RemoveSpecialChars().ToLower().Replace(" ", "").Contains(wikiName))
                                .ToList<GenericItemOption>();
                        }, options.GameData.Name, "Wikipedia: select game");


                        if (chosen != null)
                        {
                            key = ((WikipediaItemOption)chosen).Key;
                        }
                    }

                    if (key != string.Empty)
                    {
                        page = WikipediaApiCaller.GetGameData(key);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error loading data from Wikipedia");
            }

            return foundGame = new WikipediaGameMetadata(page);







            /*       var deflatedSearchGameName = options.GameData.Name.Deflate();
            List<string> platformSpecs = options.GameData.Platforms?.Where(p => p.SpecificationId != null).Select(p => p.SpecificationId).ToList();
            List<string> platformNames = options.GameData.Platforms?.Select(p => p.Name).ToList();
            foreach (var game in results)
            {
                var deflatedMatchedGameName = game.MatchedName.Deflate();
                if (!deflatedSearchGameName.Equals(deflatedMatchedGameName, StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                if (options.GameData.Platforms?.Count > 0)
                {
                    var platforms = platformUtility.GetPlatforms(game.Platform);
                    foreach (var platform in platforms)
                    {
                        if (platform is MetadataSpecProperty specPlatform && platformSpecs.Contains(specPlatform.Id))
                        {
                            return foundGame = game;
                        }
                        else if (platform is MetadataNameProperty namePlatform && platformNames.Contains(namePlatform.Name))
                        {
                            return foundGame = game;
                        }
                    }
                }
                else
                {
                    //no platforms to match, so consider a name match a success
                    return foundGame = game;
                }
            }*/

        }

        public override MetadataFile GetCoverImage(GetMetadataFieldArgs args)
        {
            string coverImageUrl = FindGame().CoverImageUrl;
            return string.IsNullOrEmpty(coverImageUrl) ? base.GetCoverImage(args) : new MetadataFile(coverImageUrl);
        }

        public override string GetName(GetMetadataFieldArgs args)
        {
            string name = FindGame().Name;
            return string.IsNullOrEmpty(name) ? base.GetName(args) : name;
        }
        public override ReleaseDate? GetReleaseDate(GetMetadataFieldArgs args)
        {
            return FindGame().ReleaseDate ?? base.GetReleaseDate(args);
        }
        public override IEnumerable<MetadataProperty> GetGenres(GetMetadataFieldArgs args)
        {
            List<MetadataProperty> genres = FindGame().Genres;
            return (genres?.Any() ?? false) ? genres : base.GetGenres(args);
        }
        public override IEnumerable<MetadataProperty> GetDevelopers(GetMetadataFieldArgs args)
        {
            List<MetadataProperty> developers = FindGame().Developers;
            return (developers?.Any() ?? false) ? developers : base.GetDevelopers(args);
        }
        public override IEnumerable<MetadataProperty> GetPublishers(GetMetadataFieldArgs args)
        {
            List<MetadataProperty> publishers = FindGame().Publishers;
            return (publishers?.Any() ?? false) ? publishers : base.GetPublishers(args);
        }
        public override IEnumerable<MetadataProperty> GetFeatures(GetMetadataFieldArgs args)
        {
            List<MetadataProperty> features = FindGame().Features;
            return (features?.Any() ?? false) ? features : base.GetFeatures(args);
        }
        public override IEnumerable<MetadataProperty> GetTags(GetMetadataFieldArgs args)
        {
            List<MetadataProperty> tags = FindGame().Tags;
            return (tags?.Any() ?? false) ? tags : base.GetTags(args);
        }
        public override IEnumerable<Link> GetLinks(GetMetadataFieldArgs args)
        {
            List<Link> links = FindGame().Links;
            return (links?.Any() ?? false) ? links : base.GetLinks(args);
        }
        public override IEnumerable<MetadataProperty> GetSeries(GetMetadataFieldArgs args)
        {
            List<MetadataProperty> series = FindGame().Series;
            return (series?.Any() ?? false) ? series : base.GetSeries(args);
        }
        public override IEnumerable<MetadataProperty> GetPlatforms(GetMetadataFieldArgs args)
        {
            List<MetadataProperty> platforms = FindGame().Platforms;
            return (platforms?.Any() ?? false) ? platforms : base.GetPlatforms(args);
        }
        public override string GetDescription(GetMetadataFieldArgs args)
        {
            string description = new DescriptionParser(FindGame().Key).Description;
            return string.IsNullOrEmpty(description) ? base.GetDescription(args) : description;
        }
    }
}