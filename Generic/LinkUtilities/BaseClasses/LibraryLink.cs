﻿using Playnite.SDK.Models;
using System;

namespace LinkUtilities.BaseClasses
{
    /// <summary>
    /// Base class for a library link
    /// </summary>
    internal abstract class LibraryLink : Link, IGameLibrary
    {
        public abstract Guid LibraryId { get; }

        public abstract bool AddLibraryLink(Game game);

        public LibraryLink() : base()
        {
        }
    }
}
