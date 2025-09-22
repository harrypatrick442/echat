using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Messages;
using Core.Messages.Messages;
using MultimediaServerCore.DataMemberNames.Messages;

namespace Chat.Messages.Client.Messages
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
            _Type = global::MessageTypes.MessageTypes.MultimediaDeletePending;
            MultimediaToken = multimediaToken;
        }
        protected DeletePendingUserMultimediaItem() { }
    }
}
