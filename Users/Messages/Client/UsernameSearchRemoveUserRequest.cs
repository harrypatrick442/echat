
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using JSON;
using Core.Port;
using Core.DataMemberNames;
using Users.Messages.Interserver;
using Core.Messages.Messages;
using MessageTypes.Internal;
using Users.DataMemberNames.Interserver.Requests;

namespace Users.Messages.Client
{
    [DataContract]
    public class UsernameSearchRemoveUserRequest : TicketedMessageBase
    {
        [JsonPropertyName(UsernameSearchRemoveUserRequestDataMemberNames.UserId)]
        [JsonInclude]
        [DataMember(Name = UsernameSearchRemoveUserRequestDataMemberNames.UserId)]
        public long UserId { get; protected set; }
        public UsernameSearchRemoveUserRequest(long userId) : 
            base(InterserverMessageTypes.UsernameSearchRemoveUser)
        {
            UserId = userId;
        }
        protected UsernameSearchRemoveUserRequest() :
            base(InterserverMessageTypes.UsernameSearchRemoveUser)
        { }

    }
}
