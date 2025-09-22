using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Messages.Messages;
using MessageTypes.Internal;
using Users.DataMemberNames.Interserver.Requests;

namespace Users.Messages.Client
{
    [DataContract]
    public class UsernameSearchAddUserRequest : TicketedMessageBase
    {
        [JsonPropertyName(UsernameSearchAddUserRequestDataMemberNames.Username)]
        [JsonInclude]
        [DataMember(Name = UsernameSearchAddUserRequestDataMemberNames.Username)]
        public string Username { get; protected set; }
        [JsonPropertyName(UsernameSearchAddUserRequestDataMemberNames.UserId)]
        [JsonInclude]
        [DataMember(Name = UsernameSearchAddUserRequestDataMemberNames.UserId)]
        public long UserId { get; protected set; }
        public UsernameSearchAddUserRequest(string username, long userId) : 
            base(InterserverMessageTypes.UsernameSearchAddUser)
        {
            Username = username;
            UserId = userId;
        }
        protected UsernameSearchAddUserRequest() :
            base(InterserverMessageTypes.UsernameSearchAddUser)
        { }

    }
}
