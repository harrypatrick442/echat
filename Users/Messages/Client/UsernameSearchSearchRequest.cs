using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Messages.Messages;
using Users.DataMemberNames.Requests;

namespace Users.Messages.Client
{
    [DataContract]
    public class UsernameSearchSearchRequest : TicketedMessageBase
    {
        [JsonPropertyName(UsernameSearchSearchRequestDataMemberNames.Str)]
        [JsonInclude]
        [DataMember(Name = UsernameSearchSearchRequestDataMemberNames.Str)]
        public string Str { get; protected set; }
        [JsonPropertyName(UsernameSearchSearchRequestDataMemberNames.MaxNEntries)]
        [JsonInclude]
        [DataMember(Name = UsernameSearchSearchRequestDataMemberNames.MaxNEntries)]
        public int MaxNEntries { get; protected set; }
        public UsernameSearchSearchRequest(string str, int maxNEntries) :
            base(MessageTypes.UsernameSearchSearch)
        {
            Str = str;
            MaxNEntries = maxNEntries;
        }
        protected UsernameSearchSearchRequest() :
            base(MessageTypes.UsernameSearchSearch)
        { }

    }
}
