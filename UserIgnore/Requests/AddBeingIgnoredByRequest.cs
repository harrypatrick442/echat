using Core.Messages.Messages;
using MessageTypes.Internal;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using UserIgnore.DataMemberNames.Interserver.Requests;

namespace UserIgnore.Requests
{
    [DataContract]
    public class AddBeingIgnoredByRequest : TicketedMessageBase
    {
        [JsonPropertyName(AddBeingIgnoredByRequestDataMemberNames.UserIdIgnoring)]
        [JsonInclude]
        [DataMember(Name = AddBeingIgnoredByRequestDataMemberNames.UserIdIgnoring)]
        public long UserIdIgnoring { get; protected set; }
        [JsonPropertyName(AddBeingIgnoredByRequestDataMemberNames.UserIdBeingIgnored)]
        [JsonInclude]
        [DataMember(Name = AddBeingIgnoredByRequestDataMemberNames.UserIdBeingIgnored)]
        public long UserIdBeingIgnored { get; protected set; }
        public AddBeingIgnoredByRequest(long userIdIgnoring, long userIdBeingIgnored)
            : base(InterserverMessageTypes.UserIgnoresAddBeingIgnoredBy)
        {
            UserIdIgnoring = userIdIgnoring;
            UserIdBeingIgnored = userIdBeingIgnored;
        }
        protected AddBeingIgnoredByRequest()
            : base(InterserverMessageTypes.UserIgnoresAddBeingIgnoredBy) { }
    }
}
