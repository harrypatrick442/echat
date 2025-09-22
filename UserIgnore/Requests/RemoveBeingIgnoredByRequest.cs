using Core.Messages.Messages;
using MessageTypes.Internal;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using UserIgnore.DataMemberNames.Interserver.Requests;

namespace UserIgnore.Requests
{
    [DataContract]
    public class RemoveBeingIgnoredByRequest : TicketedMessageBase
    {
        [JsonPropertyName(RemoveBeingIgnoredByRequestDataMemberNames.UserIdUnignoring)]
        [JsonInclude]
        [DataMember(Name = RemoveBeingIgnoredByRequestDataMemberNames.UserIdUnignoring)]
        public long UserIdUnignoring { get; protected set; }
        [JsonPropertyName(RemoveBeingIgnoredByRequestDataMemberNames.UserIdBeingUnignored)]
        [JsonInclude]
        [DataMember(Name = RemoveBeingIgnoredByRequestDataMemberNames.UserIdBeingUnignored)]
        public long UserIdBeingUnignored { get; protected set; }
        public RemoveBeingIgnoredByRequest(long userIdUnignoring, long userIdBeingUnignored)
            : base(InterserverMessageTypes.UserIgnoresRemoveBeingIgnoredBy)
        {
            UserIdUnignoring = userIdUnignoring;
            UserIdBeingUnignored = userIdBeingUnignored;
        }
        protected RemoveBeingIgnoredByRequest()
            : base(InterserverMessageTypes.UserIgnoresRemoveBeingIgnoredBy) { }
    }
}
