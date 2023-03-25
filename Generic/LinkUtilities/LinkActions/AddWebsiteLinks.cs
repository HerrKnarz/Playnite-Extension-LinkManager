﻿using KNARZhelper;
using LinkUtilities.Linker;
using Playnite.SDK;
using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LinkUtilities.LinkActions
{
    /// <summary>
    /// Class to add a link to all available websites in the Links list, if a definitive link was found.
    /// </summary>
    internal class AddWebsiteLinks : BaseClasses.LinkAction
    {
        private static AddWebsiteLinks _instance = null;
        private static readonly object _mutex = new object();

        private AddWebsiteLinks(LinkUtilities plugin) : base(plugin) => Links = new Links(Plugin);

        public static AddWebsiteLinks GetInstance(LinkUtilities plugin)
        {
            if (_instance == null)
            {
                lock (_mutex)
                {
                    if (_instance == null)
                    {
                        _instance = new AddWebsiteLinks(plugin);
                    }
                }
            }

            return _instance;
        }

        public Links Links { get; }

        public override string ProgressMessage { get; } = "LOCLinkUtilitiesProgressWebsiteLink";
        public override string ResultMessage { get; } = "LOCLinkUtilitiesDialogAddedMessage";

        private bool AddLink(Game game, BaseClasses.Link link, ActionModifierTypes actionModifier)
        {
            switch (actionModifier)
            {
                case ActionModifierTypes.Add:
                    return link.AddLink(game);
                case ActionModifierTypes.Search:
                    return link.AddSearchedLink(game);
                default:
                    return false;
            }
        }

        public override bool Execute(Game game, ActionModifierTypes actionModifier = ActionModifierTypes.None, bool isBulkAction = true)
        {
            bool result = false;

            List<BaseClasses.Link> links = null;

            switch (actionModifier)
            {
                case ActionModifierTypes.Add:
                    links = Links.Where(x => x.Settings.IsAddable == true).ToList();
                    break;
                case ActionModifierTypes.Search:
                    links = Links.Where(x => x.Settings.IsSearchable == true).ToList();
                    break;
            }

            if (isBulkAction)
            {
                foreach (BaseClasses.Link link in links)
                {
                    result = AddLink(game, link, actionModifier) || result;
                }
            }
            else
            {
                GlobalProgressOptions globalProgressOptions = new GlobalProgressOptions($"{ResourceProvider.GetString("LOCLinkUtilitiesName")}{Environment.NewLine}{ResourceProvider.GetString(ProgressMessage)}", true)
                {
                    IsIndeterminate = false
                };

                API.Instance.Dialogs.ActivateGlobalProgress((activateGlobalProgress) =>
                {
                    try
                    {
                        activateGlobalProgress.ProgressMaxValue = links.Count;

                        foreach (BaseClasses.Link link in links)
                        {
                            activateGlobalProgress.Text = $"{ResourceProvider.GetString("LOCLinkUtilitiesName")}{Environment.NewLine}{ResourceProvider.GetString(ProgressMessage)}{Environment.NewLine}{link.LinkName}";

                            if (activateGlobalProgress.CancelToken.IsCancellationRequested)
                            {
                                break;
                            }
                            else
                            {
                                result = AddLink(game, link, actionModifier) || result;
                            }

                            activateGlobalProgress.CurrentProgressValue++;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                    }
                }, globalProgressOptions);
            }

            return result;
        }
    }
}
