using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Messages.Responses;
using UsersEnums;
using Users.DataMemberNames.Interserver.Responses;

namespace Users.Messages.Interserver
{
    [DataContract]
    public class DowngradeAssociateResponse : SuccessTicketedResponse
    {
        [JsonPropertyName(DowngradeAssociateResponseDataMemberNames.AssociateTypesRemaining)]
        [JsonInclude]
        [DataMember(Name = DowngradeAssociateResponseDataMemberNames.AssociateTypesRemaining)]
        public AssociateType AssociateTypesRemaining
        {
            get;
            protected set;
        }
        public DowngradeAssociateResponse(bool success, AssociateType associateTypesRemaining, long ticket) : base(success, ticket)
        {
            AssociateTypesRemaining = associateTypesRemaining;
        }
        protected DowngradeAssociateResponse() : base()
        { 
            
        }
    }
}
