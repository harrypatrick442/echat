using DataMemberNames.Interserver.Chat.Requests;
using MessageTypes.Internal;
using Core.DataMemberNames;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Chat.Messages.Client.Requests
{
    [DataContract]
    public class SendMessageToUsersDevicesRequest
    {
        [JsonPropertyName(MessageTypeDataMemberName.Value)]
        [JsonInclude]
        [DataMember(Name = MessageTypeDataMemberName.Value)]
        public string Type
        {
            get { return InterserverMessageTypes.ChatSendMessageToUsersDevices; }
            protected set { }
        }
        private long[] _UserIds;
        [JsonPropertyName(SendMessageToUsersDevicesRequestDataMemberNames.UserIds)]
        [JsonInclude]
        [DataMember(Name = SendMessageToUsersDevicesRequestDataMemberNames.UserIds)]
        public long[] UserIds
        {
            get { return _UserIds; }
            protected set { _UserIds = value; }
        }
        private string _ReceivedMessageJsonString;
        [JsonPropertyName(SendMessageToUsersDevicesRequestDataMemberNames.ReceivedMessageJsonString)]
        [JsonInclude]
        [DataMember(Name = SendMessageToUsersDevicesRequestDataMemberNames.ReceivedMessageJsonString)]
        public string ReceivedMessageJsonString
        {
            get { return _ReceivedMessageJsonString; }
            protected set { _ReceivedMessageJsonString = value; }
        }
        public SendMessageToUsersDevicesRequest(long[] userIds, string receivedMessageJsonString)
        {
            _UserIds = userIds;
            _ReceivedMessageJsonString = receivedMessageJsonString;
        }
        protected SendMessageToUsersDevicesRequest() { }
    }
}
