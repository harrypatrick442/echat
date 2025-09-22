using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Administration.DataMemberNames.Messages;
namespace Chat
{
    [DataContract]
    public class Administrator
    {
        [JsonPropertyName(AdministratorDataMemberNames.UserId)]
        [JsonInclude]
        [DataMember(Name = AdministratorDataMemberNames.UserId)]
        public long UserId { get; protected set; }
        [JsonPropertyName(AdministratorDataMemberNames.Privilages)]
        [JsonInclude]
        [DataMember(Name = AdministratorDataMemberNames.Privilages)]
        public AdministratorPrivilages Privilages { get; set; }
        [JsonIgnore]
        public bool IsFull { get { return Privilages == AdministratorPrivilages.All; } }
        [JsonIgnore]
        public bool CanEditAdministrators { get { return (Privilages & AdministratorPrivilages.EditAdministrators) > 0; } }
        public Administrator(long userId, AdministratorPrivilages privilages) {
            UserId = userId;
            Privilages = privilages;
        }
        protected Administrator() { }

    }
}
