using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Messages.Messages;

namespace Chat.Messages.Client.Requests
{
    [DataContract]
    public class GetMyConversationSnapshotsRequest : TicketedMessageBase
    {
        public GetMyConversationSnapshotsRequest()
            : base(global::MessageTypes.MessageTypes.ChatGetConversationSnapshots)
        {

        }
    }
}
