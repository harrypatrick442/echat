using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Messages;
using Core.Messages.Messages;

namespace Chat.Messages.Client.Messages
{
    [DataContract]
    public class RemoveAdministrator : TypedMessageBase
    {
        [JsonPropertyName(RemoveAdministratorDataMemberNames.UserId)]
        [JsonInclude]
        [DataMember(Name = RemoveAdministratorDataMemberNames.UserId)]
        public long UserId{ get; protected set; }
        public RemoveAdministrator(long userId)
        {
            UserId = userId;
            _Type = MessageTypes.ChatRemoveAdministrator;
        }
    }
}
