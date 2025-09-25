using Core.Messages.Messages;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using UserIgnore.DataMemberNames.Messages;
namespace UserIgnore.Messages
{
    [DataContract]
    public class UnignoredUser:TypedMessageBase
    {
        [JsonPropertyName(UnignoredUserDataMemberNames.UserId)]
        [JsonInclude]
        [DataMember(Name = UnignoredUserDataMemberNames.UserId)]
        public long UserId { get; protected set; }
        public UnignoredUser(long userId)
            : base()
        {
            UserId = userId;
            Type = MessageTypes.UserIgnoreUnignored;
        }
        protected UnignoredUser() { }
    }
}
