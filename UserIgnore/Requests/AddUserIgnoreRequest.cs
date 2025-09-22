using Core.Messages.Messages;
using MessageTypes.Internal;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using UserIgnore.DataMemberNames.Interserver.Requests;
namespace UserIgnore.Requests
{
    [DataContract]
    public class AddUserIgnoreRequest : TicketedMessageBase
    {
        [JsonPropertyName(AddUserIgnoreRequestDataMemberNames.UserIdIgnoring)]
        [JsonInclude]
        [DataMember(Name = AddUserIgnoreRequestDataMemberNames.UserIdIgnoring)]
        public long UserIdIgnoring { get; protected set; }
        [JsonPropertyName(AddUserIgnoreRequestDataMemberNames.UserIdBeingIgnored)]
        [JsonInclude]
        [DataMember(Name = AddUserIgnoreRequestDataMemberNames.UserIdBeingIgnored)]
        public long UserIdBeingIgnored { get; protected set; }
        public AddUserIgnoreRequest(long userIdIgnoring, long userIdBeingIgnored)
            : base(InterserverMessageTypes.UserIgnoresAdd)
        {
            UserIdIgnoring = userIdIgnoring;
            UserIdBeingIgnored = userIdBeingIgnored;
        }
        protected AddUserIgnoreRequest()
            : base(InterserverMessageTypes.UserIgnoresAdd) { }
    }
}
