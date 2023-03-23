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
    internal class AddWebsiteLinks : LinkAction
    {
        /// <summary>
        /// contains all website Links that can be added.
        /// </summary>
        public Links Links;

        public override string ProgressMessage { get; } = "LOCLinkUtilitiesProgressWebsiteLink";
        public override string ResultMessage { get; } = "LOCLinkUtilitiesDialogAddedMessage";

        public AddWebsiteLinks(LinkUtilities plugin) : base(plugin)
        {
            Links = new Links(Plugin);
        }

        private bool AddLink(Game game, Linker.Link link, ActionModifierTypes actionModifier)
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

            List<Linker.Link> links = null;

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
                foreach (Linker.Link link in Links)
                {
                    result = AddLink(game, link, actionModifier) || result;
                }
            }
            else
            {
                GlobalProgressOptions globalProgressOptions = new GlobalProgressOptions(
                    $"{ResourceProvider.GetString("LOCLinkUtilitiesName")}{Environment.NewLine}{ResourceProvider.GetString(ProgressMessage)}",
                    true
                )
                {
                    IsIndeterminate = false
                };

                API.Instance.Dialogs.ActivateGlobalProgress((activateGlobalProgress) =>
                {
                    try
                    {
                        activateGlobalProgress.ProgressMaxValue = links.Count;

                        foreach (Linker.Link link in links)
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
