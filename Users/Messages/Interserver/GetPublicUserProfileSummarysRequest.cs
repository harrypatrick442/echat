using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using MessageTypes.Internal;
using Core.Messages.Messages;
using Users.DataMemberNames.Requests;

namespace Users.Messages.Interserver
{
    [DataContract]
    public class GetPublicUserProfileSummarysRequest : TicketedMessageBase
    {
        [JsonPropertyName(GetPublicUserProfileSummarysRequestDataMemberNames.UserIds)]
        [JsonInclude]
        [DataMember(Name = GetPublicUserProfileSummarysRequestDataMemberNames.UserIds)]
        public long[] UserIds
        {
            get;
            protected set;
        }
        public GetPublicUserProfileSummarysRequest(long[] userIds)
            : base(InterserverMessageTypes.UsersGetPublicUserProfileSummarys)
        {
            UserIds = userIds;
        }
        protected GetPublicUserProfileSummarysRequest()
            : base(InterserverMessageTypes.UsersGetPublicUserProfileSummarys) { }
    }
}
