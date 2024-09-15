﻿using Playnite.SDK.Models;
using System;
using System.Collections.Generic;

namespace KNARZhelper.DatabaseObjectTypes
{
    public interface IObjectType : IMetadataFieldType, IGameInfoType
    {
        int Count { get; }
        bool IsList { get; }

        bool DbObjectExists(string name);

        bool DbObjectExists(Guid id);

        bool DbObjectInUse(Guid id);

        Guid GetDbObjectId(string name);

        List<DatabaseObject> LoadAllMetadata();

        List<DatabaseObject> LoadGameMetadata(Game game);

        List<DatabaseObject> LoadUnusedMetadata(bool ignoreHiddenGames);

        bool NameExists(string name, Guid id);
    }
}