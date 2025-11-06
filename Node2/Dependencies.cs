using DependencyManagement;
using System.Reflection;
using Core;
using Core.FileSystem;
using LogServerCore;
using Logging_ClientFriendly;
namespace FileServer
{
    public static class Dependencies
    {
        public static void Initialize()
        {
            string rootDirectory = RootDirectory.Value;
            string databaseDirectory = Path.Combine(rootDirectory, "db");

            DependencyManager.AddByNames(new TupleList<string, object>
            {
               //{AlarmsDatabaseDirectory, Path.Combine(databaseDirectory, "alarms")},
               //{AlarmsIdentifierSourceJsonFilePath, Path.Combine(databaseDirectory, "identifierSource.json")},
               {Statistics.DependencyNames.StatisticsFileLoggerDirectory, Path.Combine(rootDirectory, "statistics")},

               {Statistics.DependencyNames.DefaultStatisticsDatabaseFilePath, Path.Combine(databaseDirectory, "statistics.sqlite")},
               {LogServerCore.DependencyNames.BreadcrumbIdentifiersFilePath, Path.Combine(databaseDirectory, "breadcrumbIdentifiers.json")},
               {FileServerCore.DependencyNames.ClientDirectoryPath, Path.Combine(rootDirectory, "var", "client")},
               {MaintenanceCore.DependencyNames.CurrentScheduledMaintenancesFilePath, Path.Combine(databaseDirectory, "currentScheduledMaintenances.json")},
               {CertificateManagement.DependencyNames.DontTryCertifyAgainFilePath, Path.Combine(databaseDirectory, "DontTryCertifyAgain.json")},
               {Authentication.DependencyNames.EmailToAuthenticationInfoDatabaseFilePath, Path.Combine(databaseDirectory, "auth_e")},
               {LogServerCore.DependencyNames.ErrorIdentifiersFilePath, Path.Combine(databaseDirectory, "errorIdentifiers.json")},
               {Authentication.DependencyNames.GuidToAuthenticationTokenDatabaseFilePath, Path.Combine(databaseDirectory, "guidToAuthenticationToken")},
               {NodeAssignedIdRanges.DependencyNames.IdRangesAssignedToNodesDatabaseDirectory, Path.Combine(databaseDirectory, "idRangesAssignedToNodes")},
               {NodeAssignedIdRanges.DependencyNames.NextIdFromForIdTypeDatabaseDirectory, Path.Combine(databaseDirectory, "nextIdFromForIdType")},
               //{NodeConfigurationJSONFilePath, Path.Combine(Paths.rootDirectory, "node.json")},
               //{OngoingSearchIdSourceJsonFilePath, Path.Combine(databaseDirectory, "ongoingSearchIdSource.json")},
               //{OngoingSearchIdToSearchInProgressDatabaseDirectory, Path.Combine(databaseDirectory, "ongoingSearchIdToSearchInProgress")},
               {Authentication.DependencyNames.PhoneToAuthenticationInfoDatabaseFilePath, Path.Combine(databaseDirectory, "auth_p")},
               {LogServerCore.DependencyNames.SessionIdentifiersFilePath, Path.Combine(databaseDirectory, "loggedSessionIdentifiers.json")},
               {LogServerCore.DependencyNames.SessionIdSourceDirectory, Path.Combine(databaseDirectory, "sessionIds")},
               //{SnippetIdToNameDatabaseDirectory, Path.Combine(databaseDirectory, "snippetIdToName")},
               //{SnippetIdToSnippetDatabaseDirectory, Path.Combine(databaseDirectory, "snippets")},
               //{SnippetsIdSourceDirectory, Path.Combine(databaseDirectory, "ids")},
               {LogServerCore.DependencyNames.SessionsKeyValuePairDatabaseDirectory, Path.Combine(databaseDirectory, "loggedSessions")},
               {CertificateManagement.DependencyNames.TLSCertificateJSONFilePath, Path.Combine(databaseDirectory, "tlsCertificate.json")},
               {Authentication.DependencyNames.UserIdToAuthenticationInfoDatabaseFilePath, Path.Combine(databaseDirectory, "auth_u_i")},
               {Authentication.DependencyNames.UserIdToAuthenticationTokenDatabaseFilePath, Path.Combine(databaseDirectory, "userIdToAuthenticationToken")},
               //{UserIdToDeviceStatesDatabaseDirectory, Path.Combine(databaseDirectory, "userIdToDeviceStates")},
               {Authentication.DependencyNames.UsernameToAuthenticationInfoDatabaseFilePath, Path.Combine(databaseDirectory, "auth_u_n")},
               {UserRouting.DependencyNames.UserRoutingTableDatabaseDirectory, Path.Combine(databaseDirectory, "userRoutingTable")},
               //{UsersWithSnippetOpenDatabaseDirectory, Path.Combine(databaseDirectory, "usersWithSnippetOpen")},
               
            }.ToList());
        }

    }
}
