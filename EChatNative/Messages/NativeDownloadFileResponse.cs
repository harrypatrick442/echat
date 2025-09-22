using Core.Messages.Responses;
using Native.DataMemberNames.Responses;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
namespace EChatNative.Messages
{
    [DataContract]
	public class NativeDownloadFileResponse: TicketedResponseMessageBase
    {
        [JsonPropertyName(NativeDownloadFileResponseDataMemberNames.DirectoryPath)]
        [JsonInclude]
        [DataMember(Name = NativeDownloadFileResponseDataMemberNames.DirectoryPath)]
        public string DirectoryPath
        {
            get;
            protected set;
        }
        public NativeDownloadFileResponse(string directoryPath, long ticket):base(ticket)
        {
            DirectoryPath = directoryPath;
        }
    }
}
