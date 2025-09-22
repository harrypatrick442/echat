using Core.Messages.Messages;
using Core.DataMemberNames;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Users.Messages.Interserver
{
    [DataContract]
    public class GetUserProfileSummarysResponse : TicketedMessageBase
    {
        private long[] _UserIdsCouldNotGet;
        [JsonPropertyName(GetUserProfileSummarysResponseDataMemberNames.UserIdsCouldNotGet)]
        [JsonInclude]
        [DataMember(Name = GetUserProfileSummarysResponseDataMemberNames.UserIdsCouldNotGet)]
        public long[] UserIdsCouldNotGet
        {
            get { return _UserIdsCouldNotGet; }
            protected set { _UserIdsCouldNotGet = value; }
        }
        private UserProfileSummary[] _UserProfileSummarys;
        [JsonPropertyName(GetUserProfileSummarysResponseDataMemberNames.UserProfileSummarys)]
        [JsonInclude]
        [DataMember(Name = GetUserProfileSummarysResponseDataMemberNames.UserProfileSummarys)]
        public UserProfileSummary[] UserProfileSummarys
        {
            get { return _UserProfileSummarys; }
            protected set { _UserProfileSummarys = value; }
        }
        public GetUserProfileSummarysResponse(
            UserProfileSummary[] userProfileSummarys, long ticket) : base(TicketedMessageType.Ticketed)
        {
            _UserProfileSummarys = userProfileSummarys;
            _Ticket = ticket;
        }
        protected GetUserProfileSummarysResponse() : base(TicketedMessageType.Ticketed)
        { }
    }
}
