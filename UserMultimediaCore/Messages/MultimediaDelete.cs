using Core.Messages.Messages;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using MultimediaServerCore.DataMemberNames.Messages;

namespace MultimediaServerCore.Requests
{
    [DataContract]
    public class MultimediaDelete : TicketedMessageBase
    {
        [JsonPropertyName(MultimediaDeleteDataMemberNames.UserId)]
        [JsonInclude]
        [DataMember(Name = MultimediaDeleteDataMemberNames.UserId)]
        public long UserId { get; protected set; }
        [JsonPropertyName(MultimediaDeleteDataMemberNames.MultimediaToken)]
        [JsonInclude]
        [DataMember(Name = MultimediaDeleteDataMemberNames.MultimediaToken)]
        public string MultimediaToken { get; protected set; }
        public MultimediaDelete(long userId, string multimediaToken) 
            : base(global::MessageTypes.MessageTypes.MultimediaDelete)
        {
            UserId = userId;
            MultimediaToken = multimediaToken;
        }
        protected MultimediaDelete()
            : base(global::MessageTypes.MessageTypes.MultimediaDelete) { }
    }
}
