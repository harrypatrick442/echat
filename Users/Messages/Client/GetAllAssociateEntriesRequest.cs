using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.DataMemberNames;
using Core.Messages.Messages;
using Users.DataMemberNames.Requests;
using Users.DataMemberNames.Responses;

namespace Users.Messages.Client
{
    [DataContract]
    public class GetAllAssociateEntriesRequest:TicketedMessageBase
    {
        [JsonPropertyName(GetAllAssociateEntriesRequestDataMemberNames.MyUserId)]
        [JsonInclude]
        [DataMember(Name = GetAllAssociateEntriesRequestDataMemberNames.MyUserId)]

        public long MyUserId { get; protected set; }
        public GetAllAssociateEntriesRequest(long myUserId):base(global::MessageTypes.MessageTypes.UsersGetAllAssociateEntries){
            MyUserId = myUserId;
        }
        protected GetAllAssociateEntriesRequest():base(global::MessageTypes.MessageTypes.UsersGetAllAssociateEntries) { }
    }
}
