﻿using KNARZhelper;

using Playnite.SDK.Models;

namespace LinkUtilities.Linker
{
    /// <summary>
    /// Adds a link to Gamer Guides.
    /// </summary>
    internal class LinkGamerGuides : BaseClasses.Link
    {
        public override string LinkName { get; } = "Gamer Guides";
        public override string BaseUrl { get; } = "https://www.gamerguides.com/";

        // Gamer Guides Links need the game name in lowercase without special characters and hyphens instead of white spaces.
        public override string GetGamePath(Game game, string gameName = null)
            => (gameName ?? game.Name).RemoveSpecialChars()
                .Replace("_", " ")
                .CollapseWhitespaces()
                .Replace(" ", "-")
                .ToLower();

        public LinkGamerGuides() : base()
        {
        }
    }
}