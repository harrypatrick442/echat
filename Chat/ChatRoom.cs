using Chat.Messages.Client;
using Core.Interfaces;
using Core.InterserverComs;
using Core.Timing;
using JSON;
using Logging;
using System.Text;
using Chat.MessagesHandler;
using Chat.Messages.Client.Messages;
using Chat.Messages.Client.Requests;
using Core.Messages.Responses;
using Chat.Messages.Client.Responses;
using Core.NativeExtensions;
using Core.DAL;
using HashTags;
using GlobalConstants;
using Core.Messages;
using MultimediaCore;
using MultimediaServerCore.Enums;
using MultimediaServerCore;
using MultimediaServerCore.Requests;
using System.Linq;
using Chat.Endpoints;
using Chat.DataMemberNames.Messages;
using HashTags.Messages;
using HashTags.Enums;
using Core.DataMemberNames;
namespace Chat
{
    public class ChatRoom
    {
        private const int N_CLIENT_ENDPOINTS_TO_SEND_TO_PER_THREAD = 10;
        public event EventHandler OnDisposed;
        private HashSet<ChatRoomClientEndpoint> _ClientEndpoints
            = new HashSet<ChatRoomClientEndpoint>();
        private Dictionary<long, List<ChatRoomClientEndpoint>> _MapUserIdToClientEndpoints
            = new Dictionary<long, List<ChatRoomClientEndpoint>>();
        private ChatRoomMessagesHandlerBase _ChatRoomMessagesHandler;
        private List<long> _OnlineRecentlyFirstBuffer = new List<long>();
        private long[] _OnlineRecentlySecondBuffer = new long[0];
        private string _OnlineRecentlyFirstBufferSerialized, 
            _OnlineRecentlySecondBufferSerialized;
        private long _OnlineRecentlyLastBufferSwitch;
        private bool _Disposed = false;
        public bool Disposed {
            get
            {
                lock (_ClientEndpoints) {
                    return _Disposed;
                }
            }
        }
        private long _ConversationId;
        public long ConversationId { get { return _ConversationId; } }
        private ChatRoomInfo _Info;
        private readonly object _InfoLock = new object();
        public ChatRoomInfo Info
        {
            get
            {
                lock (_InfoLock)
                {
                    return _Info;
                }
            }
        }
        private string _InfoSerialized;
        public string InfoSerialized
        {
            get
            {
                lock (_InfoLock)
                {
                    if (_InfoSerialized == null)
                        _InfoSerialized = _Info.SerializeForClient();
                    return _InfoSerialized;
                }
            }
        }
        public ChatRoom(ChatRoomInfo chatRoomInfo)
        {
            _Info = chatRoomInfo;
            _ConversationId = _Info.ConversationId;
            switch (chatRoomInfo.HistoryType) {
                case ConversationHistoryType.Overflowing:
                    _ChatRoomMessagesHandler = new ChatRoomMessagesHandler_Overflowing(chatRoomInfo.ConversationId);
                    break;
                case ConversationHistoryType.FullHistory:
                    _ChatRoomMessagesHandler = new ChatRoomMessagesHandler_FullHistory(ConversationType.PublicChatroom, chatRoomInfo.ConversationId);
                    break;
                default:
                    throw new NotImplementedException();
            }
            MostActiveChatRoomsWatcher.Instance.WatchRoomIfElegible(this);
        }
        public void SwitchOnlineRecentlyBuffers(long switchTimeMilliseconds) {
            lock (_ClientEndpoints)
            {
                if (!_OnlineRecentlySecondBuffer.Any() && !_OnlineRecentlyFirstBuffer.Any()) return;
                _OnlineRecentlySecondBuffer = _OnlineRecentlyFirstBuffer.ToArray();
                _OnlineRecentlyFirstBuffer = new List<long>();
                _OnlineRecentlySecondBufferSerialized = null;
                _OnlineRecentlyLastBufferSwitch = switchTimeMilliseconds;
            }
        }
        public RoomVisibility Visibility
        {
            get {
                lock (_InfoLock) {
                    return _Info.Visibility;
                }
            }
        }
        public bool HasJoinedUser(long userId) {
            lock (_InfoLock)
            {
                return _Info.JoinedUsers!=null&&_Info.JoinedUsers.Contains(userId);
            }
        }
        public InviteFailedReason? Invite(long myUserId, long otherUserId) {
            InviteFailedReason? failedReason = null;
            lock (_InfoLock)
            {
                if ((failedReason = _Info.CanInvite(myUserId, otherUserId, out bool already))
                    == null)
                {
                    if (already) return null;
                    DalChatRoomInfos.Instance.ModifyWithinLockWithSet(_ConversationId, (chatRoomInfo, save) =>
                    {
                        chatRoomInfo.AddInvite(otherUserId, myUserId);
                        save(chatRoomInfo);
                        _Info = chatRoomInfo;
                        _InfoSerialized = null;
                    });
                }
            }
            return failedReason;
        }
        public JoinFailedReason? Join(long userId) {
            JoinFailedReason? joinFailedReason = null;
            bool isUniversalAdmin = UniversalAdministrators.UserIds.Contains(userId);
            lock (_InfoLock) {
                if ((joinFailedReason = _Info.CanJoin(userId))==null) {
                    DalChatRoomInfos.Instance.ModifyWithinLock(_ConversationId,
                        (modifying) =>
                        {
                            if (!isUniversalAdmin)
                            {
                                modifying.AddJoinedUser(userId);
                            }
                            modifying.RemoveInvite(userId);
                            _Info = modifying;
                            _InfoSerialized = null;
                            return modifying;
                        }
                   );
                }
            }
            if (joinFailedReason == null)
            {
                SendToAllClients(new UserJoinedMessage(userId, TimeHelper.MillisecondsNow));
            }
            return joinFailedReason;
        }
        public void RemoveInvite(long userIdBeingInvited, long userIdInviting)
        {
            lock (_InfoLock)
            {
                DalChatRoomInfos.Instance.ModifyWithinLockWithSet(_ConversationId, (chatRoomInfo, save) =>
                {
                    if (chatRoomInfo.RemoveInvite(userIdBeingInvited, userIdInviting))
                    {
                        save(chatRoomInfo);
                        _Info = chatRoomInfo;
                    }
                });
            }
        }
        public void RemoveInvite(long userIdBeingInvited)
        {
            lock (_InfoLock)
            {
                DalChatRoomInfos.Instance.ModifyWithinLockWithSet(_ConversationId, (chatRoomInfo, save) =>
                {
                    if (chatRoomInfo.RemoveInvite(userIdBeingInvited))
                    {
                        save(chatRoomInfo);
                        _Info = chatRoomInfo;
                    }
                });
            }
        }
        public void RemoveUser(long userId, bool allowRemoveOnlyFullAdmin,
            out RemoveRoomUserFailedReason? failedReason)
        {
            lock (_InfoLock)
            {
                RemoveRoomUserFailedReason? failedReasonInternal = null;
                DalChatRoomInfos.Instance.ModifyWithinLockWithSet(_ConversationId, (chatRoomInfo, save) =>
                {
                    if (chatRoomInfo.RemoveUser(userId, allowRemoveOnlyFullAdmin,
                        out failedReasonInternal))
                    {
                        save(chatRoomInfo);
                        _Info = chatRoomInfo;
                        _InfoSerialized = null;
                    }
                    if (!chatRoomInfo.HasAtLeastOneFullAdministrator)
                    {
                        DalAbandonedRooms.Instance.Add(chatRoomInfo.ConversationId, TimeHelper.MillisecondsNow, chatRoomInfo.NUsers);
                    }
                });
                failedReason = failedReasonInternal;
            }
            if (failedReason == null)
            {
                SendToAllClients(new UserLeftMessage(userId));
            }
        }
        public void InfoChanged(bool pushToClients) {
            lock (_InfoLock) {
                _Info = DalChatRoomInfos.Instance.Get(_ConversationId);
                _InfoSerialized = null;
                if (pushToClients)
                {
                    lock (_Info)
                    {
                        SendToAllClients(new UpdateToChatRoomInfo(_Info));
                    }
                }
            }
        }
        public Administrator[]? GetAdministrators(long myUserId, out AdministratorsFailedReason? failedReason)
        {
            lock (_InfoLock)
            {
                Administrator[]? administrators = _Info.Administrators;
                Administrator? administratorMe = administrators?.Where(a => a.UserId == myUserId).FirstOrDefault();
                if (administratorMe == null) {
                    failedReason = AdministratorsFailedReason.NotAdministrator;
                    return null;
                }
                failedReason = null;
                return administrators;
            }
        }
        public AdministratorsFailedReason? SetAdministrator(long myUserId, long userId,
            AdministratorPrivilages privilages)
        {
            lock (_InfoLock)
            {
                AdministratorsFailedReason? failedReason = null;
                long[]? administratorUserIds = null;
                DalChatRoomInfos.Instance.ModifyWithinLockWithSet(_ConversationId, (roomInfo, set) =>
                {
                    if ((failedReason = roomInfo.SetAdministrator(myUserId, userId, privilages)) == null)
                    {
                        set(roomInfo);
                        _Info = roomInfo;
                        _InfoSerialized = null;//TODO needed?
                        administratorUserIds = roomInfo.AdministratorsUserIds();
                    }
                    if (!roomInfo.HasAtLeastOneFullAdministrator)
                    {
                        DalAbandonedRooms.Instance.Add(roomInfo.ConversationId, TimeHelper.MillisecondsNow, roomInfo.NUsers);
                    }
                });
                if (failedReason == null)
                {
                    SendToClients(
                        new SetAdministrator(userId, privilages), administratorUserIds);
                }
                return failedReason;
            }
        }
        public AdministratorsFailedReason? RemoveAdministrator(
            long myUserId, long userId, bool allowRemoveOnlyFullAdmin)
        {
            lock (_InfoLock)
            {
                AdministratorsFailedReason? failedReason = null;
                long[]? administratorUserIds = null;
                DalChatRoomInfos.Instance.ModifyWithinLockWithSet(_ConversationId, (roomInfo, set) =>
                {
                    administratorUserIds = roomInfo.AdministratorsUserIds();
                    if ((failedReason = roomInfo.RemoveAdministrator(myUserId, userId, allowRemoveOnlyFullAdmin)) == null)
                    {
                        set(roomInfo);
                        _Info = roomInfo;
                        _InfoSerialized = null;
                    }
                    if (!roomInfo.HasAtLeastOneFullAdministrator) {
                        DalAbandonedRooms.Instance.Add(roomInfo.ConversationId, TimeHelper.MillisecondsNow, roomInfo.NUsers);
                    }
                });
                if (failedReason == null)
                {
                    SendToClients(
                        new RemoveAdministrator(userId), administratorUserIds);
                }
                return failedReason;
            }
        }
        public AdministratorsFailedReason? BanUser(
            long myUserId, long userId)
        {
            lock (_InfoLock)
            {
                AdministratorsFailedReason? failedReason = null;
                DalChatRoomInfos.Instance.ModifyWithinLockWithSet(_ConversationId, (roomInfo, set) =>
                {
                    if ((failedReason = roomInfo.BanUser(myUserId, userId)) == null)
                    {
                        set(roomInfo);
                        _Info = roomInfo;
                    }
                    if (!roomInfo.HasAtLeastOneFullAdministrator)
                    {
                        DalAbandonedRooms.Instance.Add(roomInfo.ConversationId, TimeHelper.MillisecondsNow, roomInfo.NUsers);
                    }
                });
                if (failedReason == null)
                {
                    ChatRoomClientEndpoint[] endpoints;
                    lock (_ClientEndpoints)
                    {
                        endpoints = _ClientEndpoints.Where(e => e.UserId == userId).ToArray();
                    }
                    foreach (var endpoint in endpoints)
                    {
                        try
                        {
                            RemoveClientEndpoint(endpoint);
                        }
                        catch (Exception ex)
                        {
                            Logs.Default.Error(ex);
                        }
                    }
                }
                return failedReason;
            }
        }
        public AdministratorsFailedReason? UnbanUser(
            long myUserId, long userId)
        {
            lock (_InfoLock)
            {
                AdministratorsFailedReason? failedReason = null;
                DalChatRoomInfos.Instance.ModifyWithinLockWithSet(_ConversationId, (roomInfo, set) =>
                {
                    if ((failedReason = roomInfo.UnbanUser(myUserId, userId)) == null)
                    {
                        set(roomInfo);
                        _Info = roomInfo;
                    }
                });
                return failedReason;
            }
        }
        public void HandleChatRoomMessage(ClientMessage chatRoomMessage)
        {
            chatRoomMessage.Id = MessageIdSource.Instance.NextId();
            chatRoomMessage.SentAt = TimeHelper.MillisecondsNow;
            _ChatRoomMessagesHandler.Add(chatRoomMessage, out string jsonString);
            SendToAllClientsString(jsonString);
        }
        public void HandleLoadMessageHistory(LoadMessagesHistoryRequest request, IClientEndpointLight clientEndpoint) {
            LoadMessagesHistoryResponse response;
            _ChatRoomMessagesHandler.LoadMessagesHistory(
                    request.IdFromInclusive,
                    request.IdToExclusive,
                    request.NEntries,
                    out ClientMessage[] messages,
                    out MessageReaction[] reactions,
                    out MessageUserMultimediaItem[] userMultimediaItems);

            response = LoadMessagesHistoryResponse.Success(messages, reactions, 
                userMultimediaItems,
                ChatFailedReason.None, request.Ticket);
            clientEndpoint.SendObject(response);
        }
        public void HandleDeleteMessage(DeleteMessagesRequest request,
            IClientEndpointLight clientEndpoint, long myUserId)
        {
            bool canDeleteAnyMessage;
            lock (_InfoLock)
            {
                Administrator? administratorMe = _Info.Administrators.Where(a => a.UserId == myUserId).FirstOrDefault();
                canDeleteAnyMessage =
                    administratorMe != null &&
                        (administratorMe.Privilages & AdministratorPrivilages.DeleteMessages) > 0;
            }
            long[] deletedIds = _ChatRoomMessagesHandler.DeleteMessages(
                myUserId,
                request.MessageIds,
                canDeleteAnyMessage
            );
            DeleteMessagesResponse response = new DeleteMessagesResponse(deletedIds, request.Ticket);
            clientEndpoint.SendObject(response);
            SendToAllClients(new DeletedMessages(deletedIds, null));
        }
        public void HandleModifyMessage(ModifyMessage request,
            IClientEndpointLight clientEndpoint)
        {
            _ChatRoomMessagesHandler.ModifyMessage(request);
            SuccessTicketedResponse response = new SuccessTicketedResponse(true, request.Ticket);
            clientEndpoint.SendObject(response);
            SendToAllClients(request);
        }
        public void HandleReactToMessage(ReactToMessage reactToMesage)
        {
            _ChatRoomMessagesHandler.ReactToMessage(reactToMesage);
            SendToAllClients(reactToMesage);
        }
        public void HandleUnreactToMessage(UnreactToMessage unreactToMessage)
        {
            _ChatRoomMessagesHandler.UnreactToMessage(unreactToMessage);
            SendToAllClients(unreactToMessage);
        }
        public void HandleUpdateRoomInfo(UpdateChatRoomInfoRequest request)
        {
            lock (_InfoLock)
            {
                _InfoSerialized = null;
                ChatRoomInfo infoModified = null;
                bool changed = false;
                DalChatRoomInfos.Instance.ModifyWithinLockWithSet(_ConversationId, (roomInfo, set) =>
                {
                    if (roomInfo == null) throw new NullReferenceException(nameof(roomInfo));
                    bool tagsChanged = false;
                    if (request.TagsChanged&& roomInfo.HasPermission(request.UserId, AdministratorPrivilages.ModifyRoomTags))
                    {
                        string[] newTags = CombineNameTagsWithProvidedTags(request.Name, request.Tags);
                            HashTagsHelper.CrossCompareTags(newTags, roomInfo.Tags, out string[] tagsToRemove, out string[] tagsToAdd);
                        if (tagsToRemove != null || tagsToAdd != null)
                        {
                            if (tagsToRemove != null)
                            {
                                HashTagsMesh.Instance.DeleteTags(HashTagScopeTypes.ChatRoom, _ConversationId, null, tagsToRemove);
                                tagsChanged = true;
                            }
                            roomInfo.UpdateTags(tagsToRemove, tagsToAdd);
                            set(roomInfo);
                            infoModified = roomInfo;
                            if (tagsToAdd != null)
                            {
                                HashTagsMesh.Instance.AddTags(tagsToAdd, HashTagScopeTypes.ChatRoom, _ConversationId, null);
                                tagsChanged = true;
                            }
                        }
                    }
                    bool changedFromUpdate = roomInfo.Update(request);
                    if (changedFromUpdate) set(roomInfo);
                    infoModified = roomInfo;
                    changed = changedFromUpdate || tagsChanged;
                });
                _Info = infoModified;
                if (changed)
                {
                    lock (_Info)
                    {
                        SendToAllClientsString(Json.Serialize(new UpdateToChatRoomInfo(_Info)));
                    }
                }
            }
        }
        private string[]? CombineNameTagsWithProvidedTags(string name, string[] providedTags) {
            IEnumerable<string> nameTags = HashTagsHelper.SplitStringIntoTags(name);
            IEnumerable<string> providedTagsNormalized = HashTagsHelper
                .NormalizeRemoveIllegalCharactersAndRemoveDuplicates(providedTags);
            if (nameTags != null) {
                if (providedTagsNormalized != null)
                {
                    return nameTags
                        .Concat(
                            providedTagsNormalized
                        )
                        .GroupBy(t => t)
                        .Select(g => g.First())
                        .ToArray();
                }
                return nameTags.ToArray();
            }
            if (providedTagsNormalized != null) {
                return providedTagsNormalized.ToArray();
            }
            return null;
        }
        public MultimediaFailedReason? HandleUploadChatRoomPicture(UploadChatRoomPictureRequest request, long userId,
            out UserMultimediaItem? userMultimediaItem)
        {
            return ChatMultimediaMesh.Instance.UploadRoomPicture(
                _ConversationId, ConversationType.PublicChatroom, request.FileInfo, userId, request.XRating,
                request.Description, out userMultimediaItem);
        }
        public MultimediaFailedReason? HandleUploadMessagePicture(
            UploadMessagePictureRequest request, long userId,
            out UserMultimediaItem? userMultimediaItem)
        {
            return ChatMultimediaMesh.Instance.UploadChatRoomMessagePicture(
                _ConversationId, ConversationType.PublicChatroom, request.FileInfo, userId, request.XRating,
                request.Description, out userMultimediaItem);
        }
        public MultimediaFailedReason? HandleUploadMessageVideo(
            UploadMessageVideoRequest request, long userId, 
            out UserMultimediaItem? userMultimediaItem)
        {
            return ChatMultimediaMesh.Instance.UploadChatRoomMessageVideo(
                _ConversationId, ConversationType.PublicChatroom, request.FileInfo, userId, request.XRating,
                request.Description, out userMultimediaItem);
        }
        public string GetMessagesJSONArrayString(out string reactionsJSONArrayString,
            out string userMultimediaItemsJsonArrayString) {
            return _ChatRoomMessagesHandler.GetMessagesJSONArrayString(
                out reactionsJSONArrayString, out userMultimediaItemsJsonArrayString);
        }
        public long[] GetUsers(out string onlineRecentlyFirstBufferSerialized, 
            out string onlineRecentlySecondBufferSerialized, out long onlineRecentlyLastBufferSwitch)
        {
            ChatRoomClientEndpoint[] clientEndpoints;
            lock (_ClientEndpoints)
            {
                if (_Disposed)
                {
                    onlineRecentlyFirstBufferSerialized = null;
                    onlineRecentlySecondBufferSerialized = null;
                    onlineRecentlyLastBufferSwitch = 0;
                    return new long[0];
                }
                clientEndpoints = _ClientEndpoints.ToArray();
                if (_OnlineRecentlyFirstBufferSerialized == null)
                {
                    _OnlineRecentlyFirstBufferSerialized = Json.Serialize(_OnlineRecentlyFirstBuffer);
                }
                onlineRecentlyFirstBufferSerialized = _OnlineRecentlyFirstBufferSerialized;
                if (_OnlineRecentlySecondBufferSerialized == null) {
                    _OnlineRecentlySecondBufferSerialized = Json.Serialize(_OnlineRecentlySecondBuffer);
                }
                onlineRecentlySecondBufferSerialized = _OnlineRecentlySecondBufferSerialized;
                onlineRecentlyLastBufferSwitch = _OnlineRecentlyLastBufferSwitch;
            }
            return clientEndpoints.Select(c => c.UserId).ToArray();
        }
        public void SendToAllClients<TMessage>(TMessage message)
        {
            SendToAllClientsString(Json.Serialize(message));
        }
        private void SendToAllClientsString(string jsonString)
        {
            ChatRoomClientEndpoint[] clientEndpoints;
            lock (_ClientEndpoints)
            {
                if (_Disposed) return;
                clientEndpoints = _ClientEndpoints.ToArray();
            }
            SendToAllClients(clientEndpoints, jsonString);
        }
        public void SendToClients<TPayload>(TPayload payload, long[] userIds)
        {
            if (userIds == null) return;
            SendToClientsString(Json.Serialize(payload), new HashSet<long>(userIds));
        }
        private void SendToClientsString(string jsonString, HashSet<long> userIds)
        {
            ChatRoomClientEndpoint[] clientEndpoints;
            lock (_ClientEndpoints)
            {
                if (_Disposed) return;
                clientEndpoints = _ClientEndpoints.Where(c=>userIds.Contains(c.UserId)).ToArray();
            }
            SendToAllClients(clientEndpoints, jsonString);
        }
        private void SendToAllClients(ChatRoomClientEndpoint[] clientEndpoints, string jsonString) {

            int length = clientEndpoints.Length;
            for (int fromInclusive = 0; fromInclusive < length; fromInclusive += N_CLIENT_ENDPOINTS_TO_SEND_TO_PER_THREAD)
            {
                int maxToExclusive = fromInclusive + N_CLIENT_ENDPOINTS_TO_SEND_TO_PER_THREAD;
                int toExclusive = maxToExclusive > length ? length : maxToExclusive;
                SendToRangeOfClients(jsonString, clientEndpoints, fromInclusive, toExclusive);
            }
        }
        private void SendToRangeOfClients(string jsonString, ChatRoomClientEndpoint[] clientEndpoints, int fromInclusive, int toExclusive)
        {
            new Thread(() =>
            {
                try
                {
                    for (int i = fromInclusive; i < toExclusive; i++)
                    {
                        try
                        {
                            clientEndpoints[i].SendJSONString(jsonString);
                        }
                        catch { }
                    }
                }
                catch (Exception ex)
                {
                    Logs.Default.Error(ex);
                }
            }).Start();
        }
        public ChatRoomClientEndpoint[] GetClientEndpoints(long userId)
        {
            lock (_ClientEndpoints)
            {
                _MapUserIdToClientEndpoints.TryGetValue(userId, out List<ChatRoomClientEndpoint> clientEndpointsForUser);
                return clientEndpointsForUser?.ToArray();
            }
        }
        public ChatRoomClientEndpoint Add(long userId, 
            ClientMessageTypeMappingsHandler clientMessageTypeMappingsHandler,
            IClientEndpointLight chatRoomSocket)
        {
            ChatRoomClientEndpoint clientEndpoint = 
                new ChatRoomClientEndpoint(userId, clientMessageTypeMappingsHandler, this, chatRoomSocket);
            bool sendUserJoined = false;
            long enteredRoomTimestamp = TimeHelper.MillisecondsNow;
            lock (_ClientEndpoints)
            {
                if (_Disposed) return clientEndpoint;
                _ClientEndpoints.Add(clientEndpoint);
                if (_MapUserIdToClientEndpoints.TryGetValue(userId, out List<ChatRoomClientEndpoint> clientEndpointsForUser))
                {
                    clientEndpointsForUser.Add(clientEndpoint);
                }
                else
                {
                    sendUserJoined = true;
                    clientEndpointsForUser = new List<ChatRoomClientEndpoint> { clientEndpoint };
                    _MapUserIdToClientEndpoints[userId] = clientEndpointsForUser;
                }
            }
            SendEnteredRoomMessage(clientEndpoint, enteredRoomTimestamp);
            clientEndpoint.OnDispose += HandleClientEndpointDisposed;
            if (clientEndpoint.Disposed) {
                RemoveClientEndpoint(clientEndpoint);
            }
            if(sendUserJoined)
                new Thread(() => UserCameOnline(userId, enteredRoomTimestamp)).Start();
            return clientEndpoint;
        }
        private void SendEnteredRoomMessage(ChatRoomClientEndpoint clientEndpoint, long timestamp)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{\"");
            sb.Append(MessageTypeDataMemberName.Value);
            sb.Append("\":\"");
            sb.Append(MessageTypes.ChatEnteredRoom);
            sb.Append("\",\"");
            sb.Append(EnteredRoomMessageDataMemberNames.Messages);
            sb.Append("\":");
            sb.Append(GetMessagesJSONArrayString(out string reactionsJSONArrayString,
                out string userMultimediaItemsJsonArrayString));
            sb.Append(",\"");
            sb.Append(EnteredRoomMessageDataMemberNames.Reactions);
            sb.Append("\":");
            sb.Append(reactionsJSONArrayString == null ? "null" : reactionsJSONArrayString);
            sb.Append(",\"");
            sb.Append(EnteredRoomMessageDataMemberNames.UserMultimediaItems);
            sb.Append("\":");
            sb.Append(userMultimediaItemsJsonArrayString == null ? "null" : userMultimediaItemsJsonArrayString);
            sb.Append(",\"");
            sb.Append(EnteredRoomMessageDataMemberNames.OnlineUserIds);
            sb.Append("\":");
            sb.Append(Json.Serialize(GetUsers(out string onlineRecentlyFirstBufferSerialized, 
                out string onlineRecentlySecondBufferSerialized, out long onlineRecentlyLastBufferSwitch)));
            sb.Append(",\"");
            sb.Append(EnteredRoomMessageDataMemberNames.OnlineRecentlyFirstBufferUserIds);
            sb.Append("\":");
            sb.Append(onlineRecentlyFirstBufferSerialized);
            sb.Append(",\"");
            sb.Append(EnteredRoomMessageDataMemberNames.OnlineRecentlySecondBufferBufferUserIds);
            sb.Append("\":");
            sb.Append(onlineRecentlySecondBufferSerialized);
            sb.Append(",\"");
            sb.Append(EnteredRoomMessageDataMemberNames.OnlineRecentlyLastBufferSwitch);
            sb.Append("\":");
            sb.Append(onlineRecentlyLastBufferSwitch);
            sb.Append(",\"");
            sb.Append(EnteredRoomMessageDataMemberNames.Timestamp);
            sb.Append("\":");
            sb.Append(timestamp.ToString());
            sb.Append(",\"");
            sb.Append(EnteredRoomMessageDataMemberNames.Successful);
            sb.Append("\":");
            sb.Append("true");
            sb.Append(",\"");
            sb.Append(EnteredRoomMessageDataMemberNames.Info);
            sb.Append("\":");
            sb.Append(InfoSerialized);
            sb.Append("}");
            clientEndpoint.SendJSONString(sb.ToString());
        }
        private void HandleClientEndpointDisposed(object sender, EventArgs e) { 
            RemoveClientEndpoint((ChatRoomClientEndpoint)sender);
        }
        private void RemoveClientEndpoint(ChatRoomClientEndpoint clientEndpoint) {
            clientEndpoint.OnDispose-= HandleClientEndpointDisposed;
            bool demapForUserMeaningUserLeftOnAllDevices = false;
            long leftTimestamp = TimeHelper.MillisecondsNow;
            lock (_ClientEndpoints) {
                _ClientEndpoints.Remove(clientEndpoint);
                _OnlineRecentlyFirstBuffer.Add(clientEndpoint.UserId);
                if (_MapUserIdToClientEndpoints.TryGetValue(clientEndpoint.UserId, out List<ChatRoomClientEndpoint> clientEndpointsForUser))
                {
                    clientEndpointsForUser.Remove(clientEndpoint);
                    demapForUserMeaningUserLeftOnAllDevices = !clientEndpointsForUser.Any();
                    if(demapForUserMeaningUserLeftOnAllDevices)
                        _MapUserIdToClientEndpoints.Remove(clientEndpoint.UserId);
                }
            }
            clientEndpoint.Dispose();
            if(demapForUserMeaningUserLeftOnAllDevices)
                new Thread(() => UserWentOffline(clientEndpoint.UserId, leftTimestamp)).Start();
        }
        private void UserWentOffline(long userId, long timestamp)
        {
            try
            {
                SendToAllClients(new UserWentOfflineMessage(userId, timestamp));
                Info.NUsers = _ClientEndpoints.Count;
            }
            catch (Exception ex) 
            {
                Logs.Default.Error(ex);
            }
        }
        private void UserCameOnline(long userId, long timestamp)
        {
            try
            {
                SendToAllClients(new UserCameOnlineMessage(userId, timestamp));
                Info.NUsers = _ClientEndpoints.Count;
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
            }
        }
        ~ChatRoom()
        {
            Dispose();
        }
        public void Dispose()
        {
            ChatRoomClientEndpoint[] clientEndpoints;
            EventHandler onDisposed;
            lock (_ClientEndpoints) { 
                if(_Disposed) return;
                _Disposed = true;
                onDisposed = OnDisposed;
                clientEndpoints = _ClientEndpoints.ToArray();
                _ClientEndpoints.Clear();
            }
            foreach (ChatRoomClientEndpoint clientEndpoint in clientEndpoints)
            {
                clientEndpoint.OnDispose -= HandleClientEndpointDisposed;
                clientEndpoint.Dispose();
            }
            onDisposed?.Invoke(this, EventArgs.Empty);
            _ChatRoomMessagesHandler?.FlushMessagesIfChanged();
        }
    }
}