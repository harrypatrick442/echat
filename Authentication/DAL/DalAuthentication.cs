using System;
using Core.DTOs;
using Core.Enums;
using Core.Exceptions;
using Core.Ids;
using KeyValuePairDatabases;
using KeyValuePairDatabases.Enums;
using DependencyManagement;
using Initialization.Exceptions;

namespace Authentication.DAL
{
    public class DalAuthentication: DalAuthenticationBase, IDalAuthentication
    {
        private static DalAuthentication _Instance;
        public static DalAuthentication Instance
        {
            get
            {
                if (_Instance == null)
                    throw new NotInitializedException(nameof(DalAuthentication));
                return _Instance;
            }
        }
        public static DalAuthentication Initialize(int authenticationNodeId) {
                if (_Instance != null)
                    throw new AlreadyInitializedException(nameof(DalAuthentication));
                _Instance = new DalAuthentication(authenticationNodeId);
                return _Instance;
        }
        private DalAuthentication(int authenticationNodeId) :base(
            )
        {
            FixedNodeIdentifierToNodeId identifierToNodeId = new FixedNodeIdentifierToNodeId(
                authenticationNodeId);
            DalAuthenticationLocal dalAuthenticationLocal = DalAuthenticationLocal.Instance;
            _UserIdToAuthenticationInfoKeyValuePairOnDiskDatabase
            = new KeyValuePairDatabaseMesh<long, AuthenticationInfo>(
                DatabaseIdentifier.UserIdToAuthenticationInfo.Int(), 
                OnDiskDatabaseType.Sqlite,
                new OnDiskDatabaseParams
                {
                    RootDirectory = DependencyManager.GetString(DependencyNames.UserIdToAuthenticationInfoDatabaseFilePath)
                },
                new IdentifierLock<long>(),
                identifierToNodeId,
                localDatabase: dalAuthenticationLocal.UserIdToAuthenticationInfoKeyValuePairOnDiskDatabase);

            _EmailToAuthenticationInfoKeyValuePairOnDiskDatabase
            = new KeyValuePairDatabaseMesh<string, AuthenticationInfo>(
                DatabaseIdentifier.EmailToAuthenticationInfo.Int(), 
                OnDiskDatabaseType.Sqlite,
                new OnDiskDatabaseParams
                {
                    RootDirectory = DependencyManager.GetString(DependencyNames.EmailToAuthenticationInfoDatabaseFilePath)
                }, new IdentifierLock<string>(), 
                identifierToNodeId,
                localDatabase: dalAuthenticationLocal.EmailToAuthenticationInfoKeyValuePairOnDiskDatabase);

           _UsernameToAuthenticationInfoKeyValuePairOnDiskDatabase
            = new KeyValuePairDatabaseMesh<string, AuthenticationInfo>(
                DatabaseIdentifier.UsernameToAuthenticationInfo.Int(), 
                OnDiskDatabaseType.Sqlite,
                new OnDiskDatabaseParams
                {
                    RootDirectory = DependencyManager.GetString(DependencyNames.UsernameToAuthenticationInfoDatabaseFilePath)
                }, new IdentifierLock<string>(), 
                identifierToNodeId,
                localDatabase: dalAuthenticationLocal.UsernameToAuthenticationInfoKeyValuePairOnDiskDatabase);

         _PhoneToAuthenticationInfoKeyValuePairOnDiskDatabase
            = new KeyValuePairDatabaseMesh<string, AuthenticationInfo>(
                DatabaseIdentifier.PhoneToAuthenticationInfo.Int(), 
                OnDiskDatabaseType.Sqlite,
                new OnDiskDatabaseParams
                {
                    RootDirectory = DependencyManager.GetString(DependencyNames.PhoneToAuthenticationInfoDatabaseFilePath)
                }, 
                new IdentifierLock<string>(), 
                identifierToNodeId,
                localDatabase: dalAuthenticationLocal.PhoneToAuthenticationInfoKeyValuePairOnDiskDatabase);

         _UserIdToAuthenticationTokensKeyValuePairOnDiskDatabase
            = new KeyValuePairDatabaseMesh<long, AuthenticationTokens>(
                DatabaseIdentifier.UserIdToAuthentication.Int(),
                OnDiskDatabaseType.Sqlite,
                new OnDiskDatabaseParams
                {
                    RootDirectory = DependencyManager.GetString(DependencyNames.UserIdToAuthenticationTokenDatabaseFilePath)
                }, new IdentifierLock<long>(),
                identifierToNodeId,
                localDatabase: dalAuthenticationLocal.UserIdToAuthenticationTokensKeyValuePairOnDiskDatabase);

         _GuidToAuthenticationTokenKeyValuePairOnDiskDatabase
            = new KeyValuePairDatabaseMesh<string, AuthenticationToken>(
                DatabaseIdentifier.GuidToAuthenticationToken.Int(),
                OnDiskDatabaseType.Sqlite,
                new OnDiskDatabaseParams
                {
                    RootDirectory = DependencyManager.GetString(DependencyNames.GuidToAuthenticationTokenDatabaseFilePath)
                }, new IdentifierLock<string>(),
                identifierToNodeId,
                localDatabase: dalAuthenticationLocal.GuidToAuthenticationTokenKeyValuePairOnDiskDatabase
            );//TODO on disk only
    }
        
    }
}