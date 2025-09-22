using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using MessageTypes.Internal;
using Core.Messages.Messages;

namespace Users.Messages.Interserver
{
    [DataContract]
    public class GetUserProfileSummarysRequest : TicketedMessageBase
    {
        private Associate[] _Associates;
        [JsonPropertyName(GetUserProfileSummarysRequestDataMemberNames.Associates)]
        [JsonInclude]
        [DataMember(Name = GetUserProfileSummarysRequestDataMemberNames.Associates)]
        public Associate[] Associates
        {
            get { return _Associates; }
            protected set { _Associates = value; }
        }
        public GetUserProfileSummarysRequest(Associate[] associates)
            : base(InterserverMessageTypes.UsersGetUserProfileSummarys)
        {
            _Associates = associates;
        }
        protected GetUserProfileSummarysRequest()
            : base(InterserverMessageTypes.UsersGetUserProfileSummarys) { }
    }
}
