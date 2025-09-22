using Core.Messages.Messages;
using MessageTypes.Internal;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using UserIgnore.DataMemberNames.Interserver.Requests;

namespace UserIgnore.Requests
{
    [DataContract]
    public class RemoveUserIgnoreRequest : TicketedMessageBase
    {
        [JsonPropertyName(RemoveUserIgnoreRequestDataMemberNames.UserIdUnignoring)]
        [JsonInclude]
        [DataMember(Name = RemoveUserIgnoreRequestDataMemberNames.UserIdUnignoring)]
        public long UserIdUnignoring { get; protected set; }
        [JsonPropertyName(RemoveUserIgnoreRequestDataMemberNames.UserIdBeingUnignored)]
        [JsonInclude]
        [DataMember(Name = RemoveUserIgnoreRequestDataMemberNames.UserIdBeingUnignored)]
        public long UserIdBeingUnignored { get; protected set; }
        public RemoveUserIgnoreRequest(long userIdUnignoring, long userIdBeingUnignored)
            : base(InterserverMessageTypes.UserIgnoresRemove)
        {
            UserIdUnignoring = userIdUnignoring;
            UserIdBeingUnignored = userIdBeingUnignored;
        }
        protected RemoveUserIgnoreRequest()
            : base(InterserverMessageTypes.UserIgnoresRemove) { }
    }
}
