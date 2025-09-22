using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Messages.Messages;
using MessageTypes.Internal;
using MultimediaServerCore.Messages;

namespace Chat.Messages.Client.Messages
{
    [DataContract]
    public class UpdatePendingUserMultimediaItemStatus : TypedMessageBase
    {
        [JsonPropertyName(UpdatePendingUserMultimediaItemStatusDataMemberNames.StatusUpdate)]
        [JsonInclude]
        [DataMember(Name = UpdatePendingUserMultimediaItemStatusDataMemberNames.StatusUpdate)]
        public MultimediaStatusUpdate StatusUpdate{ get; protected set; }
        public UpdatePendingUserMultimediaItemStatus(MultimediaStatusUpdate statusUpdate)
        {
            StatusUpdate = statusUpdate;
            _Type = InterserverMessageTypes.ChatUpdatePendingUserMultimediaItemStatus;
        }
        protected UpdatePendingUserMultimediaItemStatus() { }
    }
}
