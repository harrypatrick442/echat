using Core.Messages.Messages;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using MessageTypes.Internal;
using MultimediaServerCore.DataMemberNames.Requests;

namespace MultimediaServerCore.Requests
{
    [DataContract]
    public class DeleteMultimediaRequest : TicketedMessageBase
    {
        [JsonPropertyName(DeleteMultimediaRequestDataMemberNames.RawMultimediaTokens)]
        [JsonInclude]
        [DataMember(Name = DeleteMultimediaRequestDataMemberNames.RawMultimediaTokens)]
        public string[] RawMultimediaTokens { get; protected set; }
        public DeleteMultimediaRequest(string[] rawMultimediaTokens) : base(InterserverMessageTypes.MultimediaDelete)
        {
            RawMultimediaTokens = rawMultimediaTokens;
        }
        protected DeleteMultimediaRequest() : base(InterserverMessageTypes.MultimediaDelete) { }
    }
}
