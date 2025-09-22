using Core.Exceptions;
using KeyValuePairDatabases;
using NodeAssignedIdRanges;
using Database;
using Core.Pool;
using Microsoft.Data.Sqlite;
using Logging;
using System;
using MentionsCore.Messages;
using System.Data;
using DependencyManagement;
namespace MentionsCore
{
    public static class DependencyNames
    {
        public const string 
            MentionsSqliteLocalDatabaseFilePath = "MentionsSqliteLocalDatabaseFilePath",
            MentionIdSourceDirectory = "MentionIdSourceDirectory";
    }
}