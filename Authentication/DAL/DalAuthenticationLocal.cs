using Core.DTOs;
using Core.Exceptions;
using KeyValuePairDatabases;
using KeyValuePairDatabases.Enums;
using DependencyManagement;
using Logging;

namespace Authentication.DAL
{
    public class DalAuthenticationLocal: DalAuthenticationBase
    {
        private static DalAuthenticationLocal _Instance;
        public static DalAuthenticationLocal Instance
        {
            get
            {
                if (_Instance == null)
                    throw new NotInitializedException(nameof(DalAuthenticationLocal));
                return _Instance;
            }
        }
        public static DalAuthenticationLocal Initialize() {
                if (_Instance != null)
                    throw new AlreadyInitializedException(nameof(DalAuthenticationLocal));
                _Instance = new DalAuthenticationLocal();
                return _Instance;
        }

        public IKeyValuePairDatabase<long, AuthenticationInfo>
            UserIdToAuthenticationInfoKeyValuePairOnDiskDatabase
        { get { return _UserIdToAuthenticationInfoKeyValuePairOnDiskDatabase; } }

        public IKeyValuePairDatabase<string, AuthenticationInfo>
            EmailToAuthenticationInfoKeyValuePairOnDiskDatabase
        { get { return _EmailToAuthenticationInfoKeyValuePairOnDiskDatabase; } }

        public IKeyValuePairDatabase<string, AuthenticationInfo>
            PhoneToAuthenticationInfoKeyValuePairOnDiskDatabase
        { get { return _PhoneToAuthenticationInfoKeyValuePairOnDiskDatabase; } }
        public IKeyValuePairDatabase<string, AuthenticationInfo>
            UsernameToAuthenticationInfoKeyValuePairOnDiskDatabase
        { get { return _UsernameToAuthenticationInfoKeyValuePairOnDiskDatabase; } }

        public IKeyValuePairDatabase<long, AuthenticationTokens>
            UserIdToAuthenticationTokensKeyValuePairOnDiskDatabase
        { get { return _UserIdToAuthenticationTokensKeyValuePairOnDiskDatabase; } }

        public IKeyValuePairDatabase<string, AuthenticationToken> 
            GuidToAuthenticationTokenKeyValuePairOnDiskDatabase
        { get { return _GuidToAuthenticationTokenKeyValuePairOnDiskDatabase; } }

        private DalAuthenticationLocal():base() {
        _UserIdToAuthenticationInfoKeyValuePairOnDiskDatabase
            = new KeyValuePairDatabase<long, AuthenticationInfo>(
                OnDiskDatabaseType.Sqlite,
                new OnDiskDatabaseParams
                {
                    FilePath = DependencyManager.GetString(DependencyNames.UserIdToAuthenticationInfoDatabaseFilePath),
                }, new IdentifierLock<long>());
            string str = DependencyManager.GetString(DependencyNames.EmailToAuthenticationInfoDatabaseFilePath);
            Logs.Default.Info(str);
            _EmailToAuthenticationInfoKeyValuePairOnDiskDatabase
            = new KeyValuePairDatabase<string, AuthenticationInfo>( 
                OnDiskDatabaseType.Sqlite,
                new OnDiskDatabaseParams
                {
                    FilePath = DependencyManager.GetString(DependencyNames.EmailToAuthenticationInfoDatabaseFilePath),
                }, new IdentifierLock<string>());
         _PhoneToAuthenticationInfoKeyValuePairOnDiskDatabase
            = new KeyValuePairDatabase<string, AuthenticationInfo>( 
                OnDiskDatabaseType.Sqlite,
                new OnDiskDatabaseParams
                {
                    FilePath = DependencyManager.GetString(DependencyNames.PhoneToAuthenticationInfoDatabaseFilePath),
                }, new IdentifierLock<string>());
         _UserIdToAuthenticationTokensKeyValuePairOnDiskDatabase
            = new KeyValuePairDatabase<long, AuthenticationTokens>(
                OnDiskDatabaseType.Sqlite,
                new OnDiskDatabaseParams
                {
                    FilePath = DependencyManager.GetString(DependencyNames.UserIdToAuthenticationTokenDatabaseFilePath),
                }, new IdentifierLock<long>());
         _GuidToAuthenticationTokenKeyValuePairOnDiskDatabase
            = new KeyValuePairDatabase<string, AuthenticationToken>(
                OnDiskDatabaseType.Sqlite,
                new OnDiskDatabaseParams
                {
                    FilePath = DependencyManager.GetString(DependencyNames.GuidToAuthenticationTokenDatabaseFilePath),
                }, new IdentifierLock<string>()
            );//TODO on disk only
        _UsernameToAuthenticationInfoKeyValuePairOnDiskDatabase
               = new KeyValuePairDatabase<string, AuthenticationInfo>(
                   OnDiskDatabaseType.Sqlite,
                new OnDiskDatabaseParams
                {
                    FilePath = DependencyManager.GetString(DependencyNames.UsernameToAuthenticationInfoDatabaseFilePath),
                }, new IdentifierLock<string>()
            );//TODO on disk only
    }
        
    }
}