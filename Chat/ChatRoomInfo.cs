using Chat.DataMemberNames.Messages;
using Chat.Messages.Client.Requests;
using JSON;
using Logging;
using MultimediaCore;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Chat
{
    [DataContract]
    public class ChatRoomInfo
    {
        [JsonPropertyName(ChatRoomInfoDataMemberNames.HistoryType)]
        [JsonInclude]
        [DataMember(Name = ChatRoomInfoDataMemberNames.HistoryType)]
        public ConversationHistoryType HistoryType { get; protected set; }
        [JsonPropertyName(ChatRoomInfoDataMemberNames.JoinedUsers)]
        [JsonInclude]
        [DataMember(Name = ChatRoomInfoDataMemberNames.JoinedUsers)]
        public HashSet<long> JoinedUsers { get; protected set; }
        [JsonPropertyName(ChatRoomInfoDataMemberNames.BannedUsers)]
        [JsonInclude]
        [DataMember(Name = ChatRoomInfoDataMemberNames.BannedUsers)]
        public HashSet<long> BannedUsers { get; protected set; }
        [JsonPropertyName(ChatRoomInfoDataMemberNames.InvitedUsers)]
        [JsonInclude]
        [DataMember(Name = ChatRoomInfoDataMemberNames.InvitedUsers)]
        public Dictionary<long, List<long>> InvitedUsers { get; protected set; }
        [JsonPropertyName(ChatRoomInfoDataMemberNames.CreatorUserId)]
        [JsonInclude]
        [DataMember(Name = ChatRoomInfoDataMemberNames.CreatorUserId)]
        public long CreatorUserId { get; protected set; }
        [JsonPropertyName(ChatRoomInfoDataMemberNames.Name)]
        [JsonInclude]
        [DataMember(Name = ChatRoomInfoDataMemberNames.Name)]
        public string Name { get; protected set; }
        [JsonPropertyName(ChatRoomInfoDataMemberNames.ConversationId)]
        [JsonInclude]
        [DataMember(Name = ChatRoomInfoDataMemberNames.ConversationId)]
        public long ConversationId { get; set; }
        private volatile int _NUsers;
        [JsonPropertyName(ChatRoomInfoDataMemberNames.NUsers)]
        [JsonInclude]
        [DataMember(Name = ChatRoomInfoDataMemberNames.NUsers)]
        public int NUsers { get { return _NUsers; } set { _NUsers = value; } }
        [JsonPropertyName(ChatRoomInfoDataMemberNames.MainPicture)]
        [JsonInclude]
        [DataMember(Name = ChatRoomInfoDataMemberNames.MainPicture)]
        public string MainPicture { get; protected set; }
        [JsonPropertyName(ChatRoomInfoDataMemberNames.PendingMainPicture)]
        [JsonInclude]
        [DataMember(Name = ChatRoomInfoDataMemberNames.PendingMainPicture)]
        public UserMultimediaItem PendingMainPicture { get; protected set; }
        [JsonPropertyName(ChatRoomInfoDataMemberNames.Administrators)]
        [JsonInclude]
        [DataMember(Name = ChatRoomInfoDataMemberNames.Administrators)]
        public Administrator[] Administrators { get; protected set; }
        [JsonPropertyName(ChatRoomInfoDataMemberNames.Visibility)]
        [JsonInclude]
        [DataMember(Name = ChatRoomInfoDataMemberNames.Visibility)]
        public RoomVisibility Visibility { get; protected set; }
        [JsonPropertyName(ChatRoomInfoDataMemberNames.Tags)]
        [JsonInclude]
        [DataMember(Name = ChatRoomInfoDataMemberNames.Tags)]
        public string[] Tags { get; protected set; }
        [JsonIgnore]
        public bool HasAtLeastOneFullAdministrator
        {
            get {
                lock (this)
                {
                    return HasAtLeastOneFullAdministratorUnlocked;
                }
            }
        }
        private bool HasAtLeastOneFullAdministratorUnlocked { get {
                if (Administrators == null) return false;
                return Administrators.Where(a => a.IsFull).Any();
            } }
        public ChatRoomInfo(long conversationId, string name,
            ConversationHistoryType historyType, long creatorUserId, RoomVisibility visibility)
        {
            ConversationId = conversationId;
            Name = name;
            HistoryType = historyType;
            CreatorUserId = creatorUserId;
            JoinedUsers = new HashSet<long> { creatorUserId };
            Administrators = new Administrator[] { new Administrator(creatorUserId, AdministratorPrivilages.All) };
            Visibility = visibility;
        }
        protected ChatRoomInfo() { }
        public void AddJoinedUser(long userId)
        {
            lock (this)
            {
                if (JoinedUsers == null)
                    JoinedUsers = new HashSet<long> { userId };
                JoinedUsers.Add(userId);
                InvitedUsers?.Remove(userId);
            }
        }
        public void UnjoinUser(long userId)
        {
            lock (this)
            {
                JoinedUsers?.Remove(userId);
            }
        }
        public void UsingJoinedUsers(Action<HashSet<long>> callback) {
            lock (this) {
                callback(JoinedUsers);
            }
        }
        public void UpdateTags(string[] tagsToRemove, string[] tagsToAdd)
        {
            lock (this)
            {
                IEnumerable<string> tags = Tags;
                if (tagsToRemove != null && tags != null)
                    tags = tags.Where(tag => !tagsToRemove.Contains(tag));
                if (tagsToAdd != null)
                {
                    if (tags == null)
                        tags = tagsToAdd;
                    else
                        tags = tags.Concat(tagsToAdd);
                }
                Tags = tags?.ToArray();
            }
        }
        public bool Update(UpdateChatRoomInfoRequest request)
        {
            bool changed = false;
            long userId = request.UserId;
            lock (this)
            {
                if (request.NameChanged && HasPermission(userId, AdministratorPrivilages.RenameRoom))
                {
                    changed = Name != request.Name;
                    Name = request.Name;
                }
                if (request.VisibilityChanged && HasPermission(userId, AdministratorPrivilages.ChangeRoomVisibility))
                {
                    changed |= Visibility != request.Visibility;
                    Visibility = request.Visibility;
                }
            }
            return changed;
        }
        public long[]? AdministratorsUserIds()
        {
            lock (this)
            {
                return Administrators?.Select(a=>a.UserId).ToArray();
            }
        }
        public void AddInvite(long userId, long userIdInviting)
        {
            lock (this)
            {
                if (InvitedUsers == null)
                {
                    InvitedUsers = new Dictionary<long, List<long>> { { userId, new List<long> { userIdInviting } } };
                    return;
                }
                if (InvitedUsers.TryGetValue(userId, out List<long> byUserIds))
                {
                    if (!byUserIds.Contains(userIdInviting))
                    {
                        byUserIds.Add(userIdInviting);
                    }
                    return;
                }
                InvitedUsers[userId] = new List<long> { userIdInviting };
            }
        }
        public bool RemoveInvite(long userId)
        {
            lock (this)
            {
                if (InvitedUsers != null)
                {
                    return InvitedUsers.Remove(userId);
                }
                return false;
            }
        }
        public bool RemoveInvite(long userIdBeingInvited, long userIdInviting)
        {
            lock (this)
            {
                if (InvitedUsers.TryGetValue(userIdBeingInvited, out List<long> userIdsInviting))
                {
                    bool removed = userIdsInviting.Remove(userIdInviting);
                    if (removed && !userIdsInviting.Any())
                    {
                        InvitedUsers.Remove(userIdBeingInvited);
                    }
                    return removed;
                }
                return false;
            }
        }
        public bool RemoveUser(long userId, bool allowRemoveOnlyFullAdmin, 
            out RemoveRoomUserFailedReason? failedReason)
        {
            lock (this)
            {
                if (!allowRemoveOnlyFullAdmin)
                {
                    bool iAmFullAdmin = Administrators?.Where(f => f.UserId == userId && f.IsFull).FirstOrDefault() != null;
                    if (iAmFullAdmin)
                    {
                        bool noOtherFullAdmins = Administrators.Where(f => f.IsFull && f.UserId != userId).FirstOrDefault() == null;
                        if (noOtherFullAdmins)
                        {
                            failedReason = RemoveRoomUserFailedReason.OnlyAdmin;
                            return false;
                        }
                    }
                }
                Administrators = Administrators?.Where(a => a.UserId != userId).ToArray();
                failedReason = null;
                if (JoinedUsers == null) return true;
                return JoinedUsers.Remove(userId);
            }
        }
        public void SetMainPicture(string multimediaToken)
        {
            lock (this)
            {
                MainPicture = multimediaToken;
            }
        }
        public void SetPendingMainPicture(UserMultimediaItem picture)
        {
            lock (this)
            {
                PendingMainPicture = picture;
            }
        }
        public AdministratorsFailedReason? SetAdministrator(long myUserId, long userId, AdministratorPrivilages privilages)
        {
            bool isUniversalAdministrator = UniversalAdministrators.UserIds.Contains(myUserId);
            lock (this)
            {
                if (privilages <= 0)
                {
                    return AdministratorsFailedReason.NoPrivilagesProvided;
                }
                if (!isUniversalAdministrator)
                {
                    Administrator? administratorMe = Administrators?.Where(a => a.UserId == myUserId).FirstOrDefault();
                    if (administratorMe == null)
                    {
                        return AdministratorsFailedReason.NotAdministrator;
                    }
                    if (!administratorMe.CanEditAdministrators)
                    {
                        return AdministratorsFailedReason.CannotEditAdministrators;
                    }
                }
                if (Administrators == null)
                {
                    Administrators = new Administrator[] { new Administrator(userId, privilages) };
                    return null;
                }
                Administrators = Administrators.Where(a => a.UserId != userId)
                        .Concat(new Administrator[] { new Administrator(userId, privilages) }).ToArray();
                return null;
            }
        }
        public AdministratorsFailedReason? RemoveAdministrator(long myUserId, long userId, bool allowRemoveOnlyFullAdmin)
        {
            bool isUniversalAdministrator = UniversalAdministrators.UserIds.Contains(myUserId);
            lock (this)
            {
                if (!isUniversalAdministrator)
                {
                    Administrator? administratorMe = Administrators?.Where(a => a.UserId == myUserId).FirstOrDefault();
                    if (administratorMe == null)
                    {
                        return AdministratorsFailedReason.NotAdministrator;
                    }
                    if (!administratorMe.CanEditAdministrators)
                    {
                        return AdministratorsFailedReason.CannotEditAdministrators;
                    }
                    if (Administrators == null)
                    {
                        return null;
                    }
                    if (!allowRemoveOnlyFullAdmin)
                    {
                        if (!Administrators.Where(a => a.UserId != userId && a.IsFull).Any())
                            return AdministratorsFailedReason.NoOtherAdmins;
                    }
                }
                Administrators = Administrators.Where(a => a.UserId != userId).ToArray();
                return null;
            }
        }
        public RoomSummary ToSummary()
        {
            lock (this)
            {
                return new RoomSummary(ConversationId, Name, NUsers, MainPicture);
            }
        }
        public RoomActivity ToRoomActivity()
        {
            lock (this)
            {
                return new RoomActivity(ConversationId, NUsers, Visibility);
            }
        }
        public string Serialize()
        {
            lock (this)
            {
                return Json.Serialize(this);
            }
        }



        public bool HasPermission(long userId, AdministratorPrivilages privilageRequired)
        {
            lock (this)
            {
                if (UniversalAdministrators.UserIds.Contains(userId)) return true;
                if (SystemAdministrators.GetUser(userId, out Administrator administrator))
                {
                    if ((administrator.Privilages & privilageRequired) == privilageRequired)
                    {
                        return true;
                    }
                }
                if (Administrators == null) return false;
                administrator = Administrators.Where(a => a.UserId == userId).FirstOrDefault();
                if (administrator == null) return false;
                return (administrator.Privilages & privilageRequired) == privilageRequired;
            }
        }
        public AdministratorsFailedReason? BanUser(long myUserId, long userId) {

            lock (this)
            {
                if (UniversalAdministrators.UserIds.Contains(userId)) return null;
                bool iAmUniversalAdministrator = UniversalAdministrators.UserIds.Contains(myUserId);
                if (!iAmUniversalAdministrator)
                {
                    Administrator administratorMe = Administrators?.Where(a => a.UserId == myUserId).FirstOrDefault();
                    if (administratorMe == null)
                    {
                        return AdministratorsFailedReason.NotAdministrator;
                    }
                    if ((administratorMe.Privilages & AdministratorPrivilages.BanUsers) <= 0)
                    {
                        return AdministratorsFailedReason.DontHavePrivilages;
                    }
                    Administrator administratorOtherUser = Administrators?.Where(a => a.UserId == userId).FirstOrDefault();
                    if (administratorOtherUser != null)
                    {
                        if (!administratorMe.CanEditAdministrators)
                        {
                            return AdministratorsFailedReason.DontHavePrivilages;
                        }
                        Administrators = Administrators?.Where(a => a.UserId != userId).ToArray();
                    }
                }
                JoinedUsers?.Remove(userId);
                InvitedUsers?.Remove(userId);
                if (BannedUsers == null) BannedUsers = new HashSet<long> { userId };
                else BannedUsers.Add(userId);
                return null;
            }
        }
        public AdministratorsFailedReason? UnbanUser(long myUserId, long userId)
        {
            lock (this) {
                bool iAmUniversalAdministrator = UniversalAdministrators.UserIds.Contains(myUserId);
                if (!iAmUniversalAdministrator)
                {
                    Administrator administratorMe = Administrators?.Where(a => a.UserId == myUserId).FirstOrDefault();
                    if (administratorMe == null)
                    {
                        return AdministratorsFailedReason.NotAdministrator;
                    }
                    if ((administratorMe.Privilages & AdministratorPrivilages.BanUsers) <= 0)
                    {
                        return AdministratorsFailedReason.DontHavePrivilages;
                    }
                }
                BannedUsers?.Remove(userId);
                return null;
            }
        }
        public JoinFailedReason? CanJoin(long userId)
        {
            lock (this)
            {
                bool isUniversalAdministrator = UniversalAdministrators.UserIds.Contains(userId);
                if (!isUniversalAdministrator)
                {
                    if (BannedUsers != null && BannedUsers.Contains(userId))
                    {
                        return JoinFailedReason.Banned;
                    }
                }
                if (JoinedUsers != null && JoinedUsers.Contains(userId))
                {
                    //RemoveInvite(userId);
                    return null;
                }
                if (isUniversalAdministrator)
                    return null;
                switch (Visibility)
                {
                    case RoomVisibility.Closed:
                        return JoinFailedReason.Closed;
                    case RoomVisibility.InviteOnlyByAdmins:
                        if (InvitedUsers != null
                            && InvitedUsers.TryGetValue(
                                userId, out List<long> userIdsInviting))
                        {
                            long[] adminUserIds = Administrators.Select(a => a.UserId).ToArray();
                            if (userIdsInviting?.Where(u => adminUserIds.Contains(u)).FirstOrDefault() != null)
                                break;
                        }
                        return JoinFailedReason.NotInvited;
                    case RoomVisibility.InviteOnlyByAnyone:
                        if (InvitedUsers != null && InvitedUsers.ContainsKey(userId))
                            break;
                        return JoinFailedReason.NotInvited;
                    case RoomVisibility.Public:
                        break;
                    default:
                        Logs.Default.Error($"Not implemented for {nameof(Visibility)} {Enum.GetName(typeof(RoomVisibility), Visibility)} so treating as public");
                        break;
                }
                return null;
            }
        }

        public InviteFailedReason? CanInvite(long myUserId, long otherUserId, out bool already)
        {
            lock (this)
            {
                already = false;
                if (BannedUsers != null)
                {
                    if (BannedUsers.Contains(myUserId))
                    {
                        return InviteFailedReason.IAmBanned;
                    }
                    if (BannedUsers.Contains(otherUserId))
                    {
                        return InviteFailedReason.TheyAreBanned;
                    }
                }
                if (JoinedUsers != null && JoinedUsers.Contains(otherUserId))
                {
                    already = true;
                    return null;
                }
                if (InvitedUsers != null && InvitedUsers.ContainsKey(otherUserId))
                {
                    already = true;
                    return null;
                }
                if (Visibility.Equals(InviteFailedReason.Closed))
                {
                    return InviteFailedReason.Closed;
                }
                bool iAmAdmin = Administrators != null
                && Administrators.Where(a => a.UserId == myUserId).FirstOrDefault() != null;
                if (Visibility.Equals(RoomVisibility.InviteOnlyByAnyone))
                {
                    if (!JoinedUsers.Contains(myUserId))
                    {
                        return InviteFailedReason.IAmNotMember;
                    }
                }
                else if (Visibility.Equals(RoomVisibility.InviteOnlyByAdmins))
                {
                    if (!iAmAdmin)
                    {
                        return InviteFailedReason.IAmNotAdmin;
                    }
                }
                return null;
            }
        }
        public string SerializeForClient() {
            return Json.Serialize(new ChatRoomInfo()
            {
                Name = this.Name,
                Administrators = this.Administrators,
                ConversationId = this.ConversationId,
                NUsers = this.NUsers,
                MainPicture = this.MainPicture,
                BannedUsers = this.BannedUsers,
                Visibility = this.Visibility,
                CreatorUserId = this.CreatorUserId,
                Tags = this.Tags,
                JoinedUsers = this.JoinedUsers
            }); ;
        }
    }
}
