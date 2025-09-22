using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.DataMemberNames.Messages;
using Core.Messages.Messages;

namespace Chat.Messages.Client.Messages
{
    [DataContract]
    public class SetAdministrator : TypedMessageBase
    {
        [JsonPropertyName(SetAdministratorDataMemberNames.UserId)]
        [JsonInclude]
        [DataMember(Name = SetAdministratorDataMemberNames.UserId)]
        public long UserId{ get; protected set; }
        [JsonPropertyName(SetAdministratorDataMemberNames.Privilages)]
        [JsonInclude]
        [DataMember(Name = SetAdministratorDataMemberNames.Privilages)]
        public AdministratorPrivilages Privilages { get; protected set; }
        public SetAdministrator(long userId, AdministratorPrivilages privilages)
        {
            UserId = userId;
            Privilages = privilages;
            _Type = global::MessageTypes.MessageTypes.ChatSetAdministrator;
        }
    }
}
