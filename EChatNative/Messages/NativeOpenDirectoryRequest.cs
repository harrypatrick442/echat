using Core.Messages.Messages;
using DataMemberNames.Client;
using Native.DataMemberNames.Requests;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
namespace EChatNative.Messages
{
    [DataContract]
	public class NativeOpenDirectoryRequest : TicketedMessageBase
    {
        [JsonPropertyName(NativeOpenDirectoryRequestDataMemberNames.DirectoryPath)]
        [JsonInclude]
        [DataMember(Name = NativeOpenDirectoryRequestDataMemberNames.DirectoryPath)]
        public string DirectoryPath { get; protected set; }
        public NativeOpenDirectoryRequest(string directoryPath):base(ClientMessageTypes.NativeOpenDirectory)
        {
            DirectoryPath = directoryPath;
        }
    }
}
