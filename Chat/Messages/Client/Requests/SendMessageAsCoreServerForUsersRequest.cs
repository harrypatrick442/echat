using System;
using Core.DataMemberNames;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using DataMemberNames.Interserver.Chat.Requests;
using MessageTypes.Internal;
using Chat.Messages.Client.Messages;

namespace Chat.Messages.Client.Requests
{
    [DataContract]
    public class SendMessageAsCoreServerForUsersRequest
    {
        [JsonPropertyName(MessageTypeDataMemberName.Value)]
        [JsonInclude]
        [DataMember(Name = MessageTypeDataMemberName.Value)]
        public string Type
        {
            get { return InterserverMessageTypes.ChatSendMessageAsCoreServerForUsers; }
            protected set { }
        }
        [JsonPropertyName(SendMessageAsCoreServerForUsersRequestDataMemberNames.ReceivedMessage)]
        [JsonInclude]
        [DataMember(Name = SendMessageAsCoreServerForUsersRequestDataMemberNames.ReceivedMessage)]
        public ClientMessage ReceivedMessage
        {
            get;
            protected set;
        }
        [JsonPropertyName(SendMessageAsCoreServerForUsersRequestDataMemberNames.UserIds)]
        [JsonInclude]
        [DataMember(Name = SendMessageAsCoreServerForUsersRequestDataMemberNames.UserIds)]
        public long[] UserIds
        {
            get;
            protected set;
        }
        public SendMessageAsCoreServerForUsersRequest(long[] userIds, ClientMessage receivedMessage)
        {
            UserIds = userIds;
            ReceivedMessage = receivedMessage;
        }
        protected SendMessageAsCoreServerForUsersRequest() { }
    }
}
