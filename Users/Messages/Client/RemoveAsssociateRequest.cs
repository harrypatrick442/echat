using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Messages.Messages;
using Users.DataMemberNames.Requests;

namespace Users.Messages.Client
{
    [DataContract]
    public class RemoveAsssociateRequest : TicketedMessageBase
    {
        private long _OtherUserId;
        [JsonPropertyName(RemoveAssociateRequestDataMemberNames.OtherUserId)]
        [JsonInclude]
        [DataMember(Name = RemoveAssociateRequestDataMemberNames.OtherUserId)]
        public long OtherUserId { get { return _OtherUserId; } protected set { _OtherUserId = value; } }
        public RemoveAsssociateRequest(long otherUserId) : base(global::MessageTypes.MessageTypes.UsersRemoveAssociate)
        {
            _OtherUserId = otherUserId;
        }
        protected RemoveAsssociateRequest() : base(global::MessageTypes.MessageTypes.UsersRemoveAssociate)
        { }

    }
}
