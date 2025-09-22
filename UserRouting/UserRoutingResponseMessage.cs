using Core.Messages.Responses;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace UserRouting
{
    [DataContract]
    public class UserRoutingResponseMessage: TicketedResponseMessageBase
    {
        private bool _Success;
        [JsonPropertyName(UserRoutingResponseMessageDataMemberNames.Success)]
        [JsonInclude]
        [DataMember(Name = UserRoutingResponseMessageDataMemberNames.Success, EmitDefaultValue =true)]
        public bool Success { get { return _Success; } protected set { _Success = value; } }
        private UserRoutingTableEntry _UserRoutingTableEntry;
        [JsonPropertyName(UserRoutingResponseMessageDataMemberNames.UserRoutingTableEntry)]
        [JsonInclude]
        [DataMember(Name = UserRoutingResponseMessageDataMemberNames.UserRoutingTableEntry)]
        public UserRoutingTableEntry UserRoutingTableEntry { get { return _UserRoutingTableEntry; }protected set { _UserRoutingTableEntry = value; } }

        public UserRoutingResponseMessage(bool success, 
            UserRoutingTableEntry userRoutingTableEntry, long ticket) : base(ticket)
        {
            _Success = success;
            _UserRoutingTableEntry = userRoutingTableEntry;
        }
        protected UserRoutingResponseMessage() : base(0) { }
    }
}
