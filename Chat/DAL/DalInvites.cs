using Chat.Interfaces;
using Microsoft.Data.Sqlite;
using Core.Enums;
using Chat.Messages.Client.Messages;
using Chat.Messages.Client;
using System.Text;
using SQLite;
using System.Data;
using MultimediaCore;
using UsersEnums;
using MultimediaServerCore.Enums;
using KeyValuePairDatabases;
using Core.Exceptions;
using KeyValuePairDatabases.Enums;
using DependencyManagement;
using Chat;
using Core.Timing;
namespace Core.DAL
{
    public class DalInvites
    {
        private static DalInvites _Instance;
        public static DalInvites Initialize()
        {
            if (_Instance != null)
                throw new AlreadyInitializedException(nameof(DalInvites));
            _Instance = new DalInvites();
            return _Instance;
        }
        public static DalInvites Instance { 
            get {
                if (_Instance == null)
                    throw new NotInitializedException(nameof(DalInvites));
                return _Instance;
            } 
        }
        private KeyValuePairDatabase<long, Invites> _KeyValuePairDatabaseMySentInvites;
        private KeyValuePairDatabase<long, Invites> _KeyValuePairDatabaseMyReceivedInvites;
        protected DalInvites()
        {
            _KeyValuePairDatabaseMySentInvites
            = new KeyValuePairDatabase<long, Invites>(
                OnDiskDatabaseType.Sqlite,
                new OnDiskDatabaseParams { 
                FilePath = DependencyManager.GetString(DependencyNames.SentInvitesDatabaseFilePath)
                },
                new IdentifierLock<long>()
            );
            _KeyValuePairDatabaseMyReceivedInvites
            = new KeyValuePairDatabase<long, Invites>(
                OnDiskDatabaseType.Sqlite,
                new OnDiskDatabaseParams
                {
                    FilePath = DependencyManager.GetString(DependencyNames.ReceivedInvitesDatabaseFilePath)
                },
                new IdentifierLock<long>()
            );
        }
        public Invites GetMyReceivedInvites(long myUserId)
        {
            return _KeyValuePairDatabaseMyReceivedInvites.Get(myUserId);
        }
        public void AddReceivedInvite(long conversationId, long userIdBeingInvited, long userIdInviting)
        {
            _KeyValuePairDatabaseMyReceivedInvites.ModifyWithinLock(userIdBeingInvited, (invites) => {
                if (invites == null) invites = new Invites();
                invites.Add(conversationId, userIdInviting, TimeHelper.MillisecondsNow);
                return invites;
            });
        }
        public bool RemoveReceivedInvite(long conversationId, long userIdBeingInvited, long? userIdInvitingRemoved,
            out long[] userIdsInviting)
        {
            bool removed = false;
            long[] userIdsInvitingIntenral  = null;
            _KeyValuePairDatabaseMyReceivedInvites.ModifyWithinLock(userIdBeingInvited, (invites) => {
                if (userIdInvitingRemoved == null)
                {
                    if (invites != null)
                    {
                        removed = invites.Remove(conversationId, out userIdsInvitingIntenral);
                    }
                    return invites;
                }
                if (invites != null)
                {
                    removed = invites.Remove(conversationId, (long)userIdInvitingRemoved);
                    if (removed)
                        userIdsInvitingIntenral = new long[] { (long)userIdInvitingRemoved };
                }
                return invites;
            });
            userIdsInviting = userIdsInvitingIntenral;
            return removed;
        }
        public void RemoveReceivedInvites(long conversationId, long myUserId, out long[] userIdsInviting)
        {
            long[] userIdsInvitingIntenral = null;
            _KeyValuePairDatabaseMyReceivedInvites.ModifyWithinLock(myUserId, (invites) => {
                invites?.Remove(conversationId, out userIdsInvitingIntenral);
                return invites;
            });
            userIdsInviting = userIdsInvitingIntenral;
        }
        public Invites GetMySentInvites(long myUserId)
        {
            return _KeyValuePairDatabaseMySentInvites.Get(myUserId);
        }
        public void AddSentInvite(long conversationId, long userIdBeingInvited, long userIdInviting)
        {
            _KeyValuePairDatabaseMySentInvites.ModifyWithinLock(userIdInviting, (invites) => {
                if (invites == null) invites = new Invites();
                invites.Add(conversationId, userIdBeingInvited, TimeHelper.MillisecondsNow);
                return invites;
            });
        }
        public void RemoveSentInvite(long conversationId, long userIdBeingInvited, long userIdInviting)
        {
            _KeyValuePairDatabaseMySentInvites.ModifyWithinLock(userIdInviting, (invites) => {
                invites?.Remove(conversationId, userIdBeingInvited);
                return invites;
            });
        }
    }
}