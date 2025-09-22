using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Nodes;
using MessageTypes.Internal;
using Core.Messages.Messages;

namespace UserRouting
{
    [DataContract]
    public class UserRoutingMessage:TicketedMessageBase
    {
        private UserIdSessionIds _UserIdSessionIds;
        [JsonPropertyName(UserRoutingMessageDataMemberNames.UserIdSessionIds)]
        [JsonInclude]
        [DataMember(Name = UserRoutingMessageDataMemberNames.UserIdSessionIds)]
        public UserIdSessionIds UserIdSessionIds { get { return _UserIdSessionIds; } protected set { _UserIdSessionIds = value; } }
        private long? _UserId;
        [JsonPropertyName(UserRoutingMessageDataMemberNames.UserId)]
        [JsonInclude]
        [DataMember(Name = UserRoutingMessageDataMemberNames.UserId)]
        protected long? UserIdNullable { get { return _UserId; } set { _UserId = value; } }
        public long UserId { get { return (long)_UserId; }}
        private int? _NodeId;
        [JsonPropertyName(UserRoutingMessageDataMemberNames.NodeId)]
        [JsonInclude]
        [DataMember(Name = UserRoutingMessageDataMemberNames.NodeId)]
        protected int? NodeIdNullable { get { return _NodeId; } set { _NodeId = value; } }
        public int NodeId { get { return (int)_NodeId; } }
        private long? _SessionId;
        [JsonPropertyName(UserRoutingMessageDataMemberNames.SessionId)]
        [JsonInclude]
        [DataMember(Name = UserRoutingMessageDataMemberNames.SessionId)]
        protected long? SessionIdNullable { get { return _SessionId; } set { _SessionId = value; } }
        public long SessionId { get { return (long)_SessionId; }}



        private UserRoutingTableEntry _UserRoutingTableEntry;
        [JsonPropertyName(UserRoutingMessageDataMemberNames.UserRoutingTableEntry)]
        [JsonInclude]
        [DataMember(Name = UserRoutingMessageDataMemberNames.UserRoutingTableEntry)]
        public UserRoutingTableEntry UserRoutingTableEntry { get { return _UserRoutingTableEntry; } protected set { _UserRoutingTableEntry = value; } }


        private UserRoutingTableEntry[] _UserRoutingTableEntries;
        [JsonPropertyName(UserRoutingMessageDataMemberNames.UserRoutingTableEntries)]
        [JsonInclude]
        [DataMember(Name = UserRoutingMessageDataMemberNames.UserRoutingTableEntries)]
        public UserRoutingTableEntry[] UserRoutingTableEntries { get { return _UserRoutingTableEntries; } protected set { _UserRoutingTableEntries = value; } }




        private UserRoutingOperation? _Operation;
        [JsonPropertyName(UserRoutingMessageDataMemberNames.Operation)]
        [JsonInclude]
        [DataMember(Name = UserRoutingMessageDataMemberNames.Operation)]
        protected UserRoutingOperation? OperationNullable { get { return _Operation; } set { _Operation = value; } }
        public UserRoutingOperation Operation { get { return (UserRoutingOperation)_Operation; } }
        public UserRoutingMessage(UserRoutingOperation operation, UserRoutingTableEntry userRoutingTableEntry) :base(InterserverMessageTypes.UserRoutingMessage){
            _Operation = operation;
            _UserRoutingTableEntry = userRoutingTableEntry;
            //_AtUTC = atUTC;
        }
        public UserRoutingMessage(int nodeId, long userId, UserRoutingOperation operation, long sessionId/*, long atUTC*/) : base(InterserverMessageTypes.UserRoutingMessage)
        {
            _NodeId = nodeId;
            _UserId = userId;
            _Operation = operation;
            _SessionId = sessionId;
            //_AtUTC = atUTC;
        }
        public UserRoutingMessage(UserIdSessionIds userIdSessionIds, 
            UserRoutingOperation operation, int nodeId): base(InterserverMessageTypes.UserRoutingMessage)
        {
            _UserIdSessionIds = userIdSessionIds;
            _Operation = operation;
            _NodeId = nodeId;
        }
        public UserRoutingMessage(UserRoutingTableEntry[] userRoutingTableEntries,
            UserRoutingOperation operation, int nodeId): base(InterserverMessageTypes.UserRoutingMessage)
        {
            _UserRoutingTableEntries = userRoutingTableEntries;
            _Operation = operation;
            _NodeId = nodeId;
        }
        protected UserRoutingMessage() : base(InterserverMessageTypes.UserRoutingMessage)
        { }
    }
}
