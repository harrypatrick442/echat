using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Messages.Messages;
using UsersEnums;
using Users.DataMemberNames.Requests;

namespace Users.Messages.Client
{
    [DataContract]
    public class InviteAssociateRequest : TicketedMessageBase
    {
        [JsonPropertyName(InviteAssociateRequestDataMemberNames.Email)]
        [JsonInclude]
        [DataMember(Name = InviteAssociateRequestDataMemberNames.Email)]
        public string Email { get; protected set; }
        [JsonPropertyName(InviteAssociateRequestDataMemberNames.PhoneNumber)]
        [JsonInclude]
        [DataMember(Name = InviteAssociateRequestDataMemberNames.PhoneNumber)]
        public string PhoneNumber  { get; protected set; }
        [JsonPropertyName(InviteAssociateRequestDataMemberNames.AssociateType)]
        [JsonInclude]
        [DataMember(Name = InviteAssociateRequestDataMemberNames.AssociateType)]
        public AssociateType AssociateType { get; protected set; }
        public InviteAssociateRequest(string email, string phoneNumber, AssociateType associateType) 
            : base(MessageTypes.UsersInviteAssociateByUserId) {
            Email = email;
            PhoneNumber = phoneNumber;
            AssociateType = associateType;
        }
        protected InviteAssociateRequest() : base(MessageTypes.UsersInviteAssociateByUserId)
        { }
    }
}
