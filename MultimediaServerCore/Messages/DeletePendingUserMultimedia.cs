using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Messages.Messages;
using MultimediaServerCore.DataMemberNames.Messages;

namespace MultimediaServerCore.Messages.Messages
{
    [DataContract]
    public class DeletePendingUserMultimediaItem : TypedMessageBase
    {
        [JsonPropertyName(DeletePendingUserMultimediaItemDataMemberNames.MultimediaToken)]
        [JsonInclude]
        [DataMember(Name = DeletePendingUserMultimediaItemDataMemberNames.MultimediaToken)]
        public string MultimediaToken
        {
            get;
            set;
        }
        public DeletePendingUserMultimediaItem(string multimediaToken) : base()
        {
            _Type = MessageTypes.MultimediaDeletePending;
            MultimediaToken = multimediaToken;
        }
        protected DeletePendingUserMultimediaItem() { }
    }
}
